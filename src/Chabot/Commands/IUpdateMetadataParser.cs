namespace Chabot.Commands;

public interface IUpdateMetadataParser<in TUpdate>
{
    UpdateMetadata ParseUpdateMetadata(TUpdate update);
}