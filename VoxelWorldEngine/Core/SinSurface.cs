using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;

namespace VoxelWorldEngine.Core;

public static class SinSurface
{
 // Період 256 (PI / 128).
    private const int Period = 256;
    // Логарифм від 512 (бо ми дублюємо масив для циклічності) = 9
    private const int LogK = 9; 

    // Таблиці для пошуку [рівень_деталізації][індекс]
    private static readonly float[][] _minTable;
    private static readonly float[][] _maxTable;
    
    // Таблиця логарифмів для швидкого визначення "k"
    private static readonly int[] _logs;

    static SinSurface()
    {
        // 1. Підготовка даних (Sin)
        // Ми робимо масив 512 (2 * Period), щоб легко обробляти випадки,
        // коли діапазон вилазить за 255 і починається з 0 (wrap-around).
        int size = Period * 2;
        float[] rawValues = new float[size];
        const float scale = (float)Math.PI / 128f;

        for (int i = 0; i < size; i++)
        {
            // i % Period забезпечує повторення
            rawValues[i] = (float)Math.Sin((i % Period) * scale);
        }

        // 2. Ініціалізація Sparse Table
        _minTable = new float[LogK + 1][];
        _maxTable = new float[LogK + 1][];
        _minTable[0] = rawValues; // Рівень 0 - це самі значення
        _maxTable[0] = rawValues;

        for (int k = 1; k <= LogK; k++)
        {
            int len = 1 << k; // 2, 4, 8...
            int halfLen = 1 << (k - 1);
            int count = size - len + 1;
            
            _minTable[k] = new float[count];
            _maxTable[k] = new float[count];

            for (int i = 0; i < count; i++)
            {
                _minTable[k][i] = Math.Min(_minTable[k - 1][i], _minTable[k - 1][i + halfLen]);
                _maxTable[k][i] = Math.Max(_maxTable[k - 1][i], _maxTable[k - 1][i + halfLen]);
            }
        }

        // 3. Прекалькуляція логарифмів (щоб не рахувати Math.Log2 кожен раз)
        _logs = new int[Period + 1];
        _logs[1] = 0;
        for (int i = 2; i <= Period; i++)
            _logs[i] = _logs[i / 2] + 1;
    }

    public static (float min, float max) GetSinCosRange(Vector3Int min, Vector3Int max)
    {
        // Отримуємо діапазон для X (Sin)
        // Використовуємо offset = 0
        var (minX, maxX) = Query(min.X, max.X, 0);

        // Отримуємо діапазон для Z (Cos)
        // Cos(x) = Sin(x + PI/2). Оскільки період 256, то PI/2 = 64 кроки.
        // Тому ми використовуємо ту саму таблицю Sin, але додаємо 64 до індексів.
        var (minZ, maxZ) = Query(min.Z, max.Z, 64);

        return (minX + minZ, maxX + maxZ);
    }

    // O(1) запит
    private static (float, float) Query(int start, int end, int offset)
    {
        int count = end - start + 1; // Кількість елементів (включно)

        // Якщо діапазон більше періода, ми точно захопимо глобальний мінімум і максимум
        if (count >= Period)
        {
            // Для Sin/Cos глобальні мін/макс це -1 і 1 (наближено до float)
            // Але краще взяти з таблиці, щоб було ідеально точно як у генерації
            return (_minTable[8][0], _maxTable[8][0]); // 8-й рівень покриває 256 елементів
        }

        // Нормалізація індексів до 0..255 + зсув для Cos
        // Використовуємо бітову маску для швидкого modulo 256
        int l = (start + offset) & 255;
        
        // Знаходимо степінь двійки, що влізає в наш діапазон
        int k = _logs[count];
        int len = 1 << k; // 2^k

        // Дивимось два відрізки: від початку і від кінця (вони перекриваються)
        // Завдяки тому, що ми задублювали масив до 512, нам не треба думати про перехід 255->0
        
        float min1 = _minTable[k][l];
        float max1 = _maxTable[k][l];
        
        // Правий край діапазону. 
        // Якщо l + count > 256, ми просто ліземо в другу половину масиву (256..511)
        int r = l + count - len; 
        
        float min2 = _minTable[k][r];
        float max2 = _maxTable[k][r];

        return (
            (min1 < min2) ? min1 : min2,
            (max1 > max2) ? max1 : max2
        );
    }
}