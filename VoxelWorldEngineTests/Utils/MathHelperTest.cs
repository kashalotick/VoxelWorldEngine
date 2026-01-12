using VoxelWorldEngine.Utils;

namespace VoxelWorldEngineTests.Utils;

public class MathHelperTest
{
    // ---------------------------------------------------------------------
    // Тести для FloorDiv
    // Мета: Перевірити, що ділення округлюється "вниз" (до мінус нескінченності),
    // а не "до нуля" (як стандартний оператор / у C#).
    // ---------------------------------------------------------------------

    [Test]
    [Description("Позитивне / Позитивне: Поведінка ідентична звичайному діленню.")]
    [TestCase(10, 3, ExpectedResult = 3)] // 3.33 -> 3
    [TestCase(12, 3, ExpectedResult = 4)] // 4.0  -> 4
    public int FloorDiv_PositiveNumbers(int a, int b)
    {
        return MathHelper.FloorDiv(a, b);
    }

    [Test]
    [Description("Від'ємне / Позитивне: Результат має бути меншим або рівним алгебраїчному результату.")]
    [TestCase(-10, 3, ExpectedResult = -4)] // -3.33 -> floor is -4 (C# '/' gives -3)
    [TestCase(-1, 3, ExpectedResult = -1)] // -0.33 -> floor is -1 (C# '/' gives 0)
    [TestCase(-12, 3, ExpectedResult = -4)] // -4.0  -> -4
    public int FloorDiv_NegativeDividend(int a, int b)
    {
        return MathHelper.FloorDiv(a, b);
    }

    [Test]
    [Description("Позитивне / Від'ємне: Також має округлювати вниз.")]
    [TestCase(10, -3, ExpectedResult = -4)] // -3.33 -> floor is -4
    public int FloorDiv_NegativeDivisor(int a, int b)
    {
        return MathHelper.FloorDiv(a, b);
    }

    [Test]
    [Description("Від'ємне / Від'ємне: Результат позитивний, округлення вниз.")]
    [TestCase(-10, -3, ExpectedResult = 3)] // 3.33 -> 3
    public int FloorDiv_BothNegative(int a, int b)
    {
        return MathHelper.FloorDiv(a, b);
    }

    [Test]
    public void FloorDiv_DivideByZero_ThrowsException()
    {
        Assert.Throws<DivideByZeroException>(() => MathHelper.FloorDiv(10, 0));
    }

    // ---------------------------------------------------------------------
    // Тести для Mod
    // Мета: Перевірити, що залишок завжди позитивний [0..b-1].
    // Стандартний оператор % у C# повертає залишок зі знаком діленого (наприклад -1 % 3 == -1).
    // ---------------------------------------------------------------------

    [Test]
    [Description("Звичайний модус для позитивних чисел.")]
    [TestCase(10, 3, ExpectedResult = 1)] // 10 = 3*3 + 1
    [TestCase(12, 4, ExpectedResult = 0)] // Ділиться без остачі
    [TestCase(0, 5, ExpectedResult = 0)] // 0
    public int Mod_PositiveNumbers(int a, int b)
    {
        return MathHelper.Mod(a, b);
    }

    [Test]
    [Description("Модус для від'ємних чисел має повертати позитивне значення (циклічність).")]
    // Логіка: ... -3, -2, -1, 0, 1, 2, ...
    // Mod 3:  ...  0,  1,  2, 0, 1, 2, ...
    [TestCase(-1, 3, ExpectedResult = 2)] // Стандартний % дає -1. Очікуємо 2.
    [TestCase(-2, 3, ExpectedResult = 1)]
    [TestCase(-3, 3, ExpectedResult = 0)] // -3 ділиться на 3 націло
    [TestCase(-4, 3, ExpectedResult = 2)] // Цикл повторюється
    [TestCase(-10, 3, ExpectedResult = 2)] // -10 = 3*(-4) + 2
    public int Mod_NegativeDividend(int a, int b)
    {
        return MathHelper.Mod(a, b);
    }

    [Test]
    [Description("Перевірка поведінки при великих числах.")]
    [TestCase(int.MinValue + 1, 3, ExpectedResult = 2)] // Залежить від реалізації, але має бути >= 0
    public int Mod_EdgeCases(int a, int b)
    {
        var result = MathHelper.Mod(a, b);

        // Перевіряємо загальну умову: результат має бути в межах [0, b)
        Assert.That(result, Is.GreaterThanOrEqualTo(0));
        Assert.That(result, Is.LessThan(b));

        return result;
    }

    [Test]
    public void Mod_DivideByZero_ThrowsException()
    {
        Assert.Throws<DivideByZeroException>(() => MathHelper.Mod(10, 0));
    }
}