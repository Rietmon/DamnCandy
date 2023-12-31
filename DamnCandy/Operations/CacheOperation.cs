using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DamnCandy.Handlers;
using DamnCandy.Metadatas;
using DamnCandy.Providers;
using DamnCandy.Utilities;

namespace DamnCandy.Operations
{
    public class CacheOperation
    {
        private static int NextOperationId { get; set; }

        private readonly CancellationTokenSource cancellationTokenSource;

        public int OperationId { get; } = ++NextOperationId;
        public string CacheId { get; }
    
        public ICacheProvider CacheProvider { get; }
    
        public ICacheHandler CacheHandler { get; }
    
        public CacheOperationStage Stage { get; private set; }
    
        public CacheStatus Status { get; private set; }
    
        public byte[] Data { get; private set; }
    
        public Guid Guid { get; private set; }
    
        public CacheMetadata Metadata { get; private set; }

        public float Progress => (float)Stage / 6f;
    
        public CacheOperation Parent { get; }
    
        public CacheOperation[] Children { get; private set; }
    
        public Task HandlerTask { get; private set; }
    
        public Exception Exception { get; private set; }

        private CancellationTokenSource Source => Parent != null ? Parent.cancellationTokenSource : cancellationTokenSource;
    
        public event Action OnOperationEnded
        {
            add => onOperationEnded += value;
            remove => onOperationEnded -= value;
        }
    
        private Action onOperationEnded;

        public CacheOperation(string cacheId, ICacheProvider provider, ICacheHandler handler, CacheOperation parent = null)
        {
            CacheId = cacheId;
            if (CacheId != string.Empty)
                Guid = cacheId.CreateGuid();
        
            CacheProvider = provider;
            CacheHandler = handler;
            Parent = parent;
        
            if (Parent == null)
                cancellationTokenSource = new CancellationTokenSource();
        }
    
        public void Cancel()
        {
            Status = CacheStatus.Cancelled;
            Stage = CacheOperationStage.Ended;
            Source.Cancel();
        }
    
        internal void Begin()
        {
            Status = CacheStatus.InProgress;
            HandlerTask = Task.Run(Handler, Source.Token);
        }

        private async Task Handler()
        {
            try
            {
                await ProcessCache();

                if (Guid == Guid.Empty)
                    CreateGuid();

                FileSystemUtilities.ValidatePath($"{CacheSettings.CacheDataPath}/{Guid}");

                CreateMetadata();

                await SaveBytes();
            
                Metadata.CacheDate = DateTime.UtcNow;
                Metadata.IsValid = true;
                Metadata.Save();

                if (CacheProvider.ProcessDependencies)
                    await ProcessDependencies();
            
                Status = CacheStatus.Cached;
            }
            catch (Exception e)
            {
                Exception = e;
                Status = CacheStatus.Failed;
            }

            Stage = CacheOperationStage.Ended;
            onOperationEnded();
        }

        private async Task ProcessCache()
        {
            Stage = CacheOperationStage.Downloading;
            Data = await CacheProvider.ProcessCacheAsync();
        }

        private void CreateGuid()
        {
            Stage = CacheOperationStage.CreatingGuid;
            Guid = CacheProvider.GetGuid();
        }

        private void CreateMetadata()
        {
            Stage = CacheOperationStage.CreatingMetadata;
            Metadata = new CacheMetadata
            {
                Guid = Guid,
                CacheId = CacheId
            };
            Metadata.Save();
        }

        private async Task SaveBytes()
        {
            Stage = CacheOperationStage.Saving;
            await CacheHandler.SaveBytesAsync(Guid, Data);
        }

        private async Task ProcessDependencies()
        {
            Stage = CacheOperationStage.ResolvingDependencies;
            var dependencies = CacheProvider.GetDependencies().ToList();
            Children = new CacheOperation[dependencies.Count];
            for (var i = 0; i < dependencies.Count; i++)
            {
                var operation = CacheManager.ProcessDependency(dependencies[i], this);
                Children[i] = operation;
            }

            await Task.WhenAll(Children.Select((o) => o.HandlerTask));
                
            Metadata.Dependencies = Children.Select((o) => o.Guid).ToArray();
            Metadata.IsDependenciesValid = true;
            Metadata.Save();
        }
    }
}