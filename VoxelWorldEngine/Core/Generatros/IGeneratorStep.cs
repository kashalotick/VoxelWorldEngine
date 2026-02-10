namespace VoxelWorldEngine.Core.Generatros;

public interface IGeneratorStep
{
    GenerationContext Apply(GenerationContext context);
}