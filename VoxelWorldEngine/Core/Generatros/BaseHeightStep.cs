using DotnetNoise;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;

namespace VoxelWorldEngine.Core.Generatros;

public class BaseHeightStep : IGeneratorStep
{
    public GenerationContext Apply(GenerationContext context)
    {
        return context;
    }
}