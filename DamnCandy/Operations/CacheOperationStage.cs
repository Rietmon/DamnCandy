namespace DamnCandy.Operations;

public enum CacheOperationStage
{
    NotStarted,
    Downloading,
    CreatingGuid,
    CreatingMetadata,
    Saving,
    ResolvingDependencies,
    Ended
}