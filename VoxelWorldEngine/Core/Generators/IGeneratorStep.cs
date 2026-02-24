namespace VoxelWorldEngine.Core.Generators;

public interface IGeneratorStep
{
    GenerationContext Apply(GenerationContext context);
}