using System;

namespace Chabot.Commands.Exceptions
{
    public class InvalidMetadataInheritanceTypeException : Exception
    {
        public InvalidMetadataInheritanceTypeException(Type metadataType)
            : base($"Metadata of type {metadataType.FullName} has various inheritance type")
        {
        }
    }
}