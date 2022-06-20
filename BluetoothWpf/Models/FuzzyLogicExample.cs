namespace BluetoothWpf.Models
{
    public class FuzzyLogicExample
    {
        private const int NUMBER_OF_POINT_OUT = 31; // количество точек аппроксимации выходного терма
        private const int MAX_RULE_NUMBER = 3;      // количество правил логического вывода
        private const int MAX_MARK = 10;            // максимальная оценка в баллах для качества пищи и сервиса
        private const int MAX_FPA = MAX_MARK + 1;   // максимальное число дискрет для входной ф-ции принадлежности
        private const int MAX_LP = 3;              // максимальное число термов в лингв. переменной
        
        float[,]  _serviceMemberFunc = new float[MAX_LP,MAX_FPA] {
            {1.000f, 0.882f, 0.607f, 0.325f, 0.135f, 0.044f, 0.011f, 0.002f, 0.000f, 0.000f, 0.000f},  // s_bad
            {0.044f, 0.135f, 0.325f, 0.607f, 0.882f, 1.000f, 0.882f, 0.607f, 0.325f, 0.135f, 0.044f},  // s_good
            {0.000f, 0.000f, 0.000f, 0.002f, 0.011f, 0.044f, 0.135f, 0.325f, 0.607f, 0.882f, 1.000f}   // s_fine
        };
        
        float[,]  _foodMemberFunc = new float[MAX_LP,MAX_FPA] {
            {1.000f, 1.000f, 0.667f, 0.333f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f},  // f_bad
            {0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f},  // f_good
            {0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.250f, 0.500f, 0.750f, 1.000f, 1.000f} // f_fine
        };

        private float[,] _tipsMemberFunc = new float[MAX_RULE_NUMBER, NUMBER_OF_POINT_OUT]
        {
            {
                0.000f, 0.200f, 0.400f, 0.600f, 0.800f, 1.000f, 0.800f, 0.600f, 0.400f, 0.200f, // low
                0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f,
                0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f
            },
            {
                0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, // middle
                0.000f, 0.200f, 0.400f, 0.600f, 0.800f, 1.000f, 0.800f, 0.600f, 0.400f, 0.200f,
                0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f
            },
            {
                0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, // high
                0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f,
                0.000f, 0.200f, 0.400f, 0.600f, 0.800f, 1.000f, 0.800f, 0.600f, 0.400f, 0.200f, 0.000f
            }
        };

        public FuzzyLogicExample()
        {
        }

        private float[] _outputFuzzyFunction = new float[NUMBER_OF_POINT_OUT];
        
        float MatrixDisjunction(float x, float y)
        {
            return  1 - (1 - x) * (1 - y);
        }
        
        float MatrixConjunction(float x, float y)
        {
            return x * y;
        }
        
        int CenterOfMass()
        {
            int crisp;
            float tmp1 = 0;
            float tmp2 = 0;
            
            for (int i = 0; i < NUMBER_OF_POINT_OUT; i++)
            {
                tmp1 += _outputFuzzyFunction[i];
                tmp2 += i * _outputFuzzyFunction[i];
            }
            crisp = (int)(tmp2 / tmp1);
            return crisp;
        }
        
        public int FuzzyLogic(int service, int food)
        {
            float[] u = new float[MAX_RULE_NUMBER];
            float[] itmp = new float[MAX_RULE_NUMBER];
            float tmp;
            for (int j = 0; j < NUMBER_OF_POINT_OUT; j++)
            {
                // инициализация wc
                _outputFuzzyFunction[j] = 0.0f;
            } 
            for (int j = 0; j < MAX_RULE_NUMBER; j++)
            {
                u[j] = MatrixDisjunction(_serviceMemberFunc[j,service], _foodMemberFunc[j,food]);
            }
            for (int j = 0; j < NUMBER_OF_POINT_OUT; j++)
            { 
                // вычисление выходной ф-ии принадлежности
                for (int i = 0; i < MAX_RULE_NUMBER; i++)
                { // вычисление выходов для каждого правила
                    itmp[i] = MatrixConjunction(u[i], _tipsMemberFunc[i, j]); // модель импликации u[i] - антецедент,
                    tmp = MatrixDisjunction(_outputFuzzyFunction[j], itmp[i]); // композиция
                    _outputFuzzyFunction[j] = tmp;
                }
            }
            return CenterOfMass();
        }



    }
}