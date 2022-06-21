namespace BluetoothWpf.Models
{
    public class FuzzyLogicExample
    {
        private const int NUMBER_OF_POINT_OUT = 21; // количество точек аппроксимации выходного терма
        private const int MAX_RULE_NUMBER = 4;      // количество правил логического вывода
        private const int MAX_MARK = 10;            // максимальная оценка в баллах для качества пищи и сервиса
        private const int MAX_FPA = MAX_MARK + 1;   // максимальное число дискрет для входной ф-ции принадлежности
        private const int MAX_LP = 4;              // максимальное число термов в лингв. переменной
        
        float[,]  _ageMemberFunc = new float[MAX_LP,MAX_FPA] {
            {1.000f, 1.000f, 0.500f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f},   // юный
            {0.000f, 0.000f, 1.000f, 1.000f, 0.500f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f},   // молодой
            {0.000f, 0.000f, 0.000f, 0.500f, 1.000f, 1.000f, 1.000f, 0.500f, 0.000f, 0.000f, 0.000f},   // зрелый
            {0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.500f, 1.000f, 1.000f, 1.000f, 1.000f}    // пожилой
        };
        
        float[,]  _temperamentMemberFunc = new float[MAX_LP,MAX_FPA] {
            {1.000f, 1.000f, 0.750f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f},   // сангвиник
            {0.000f, 0.000f, 0.750f, 1.000f, 1.000f, 0.750f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f},   // флегматик
            {0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.750f, 1.000f, 1.000f, 0.750f, 0.000f, 0.000f},   // меланхолик
            {0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.750f, 1.000f, 1.000f}    // холерик
        };
        
        float[,]  _attentionLevelMemberFunc = new float[MAX_LP,MAX_FPA] {
            {0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.550f, 0.800f, 1.000f},   // низкий
            {0.000f, 0.000f, 0.000f, 0.000f, 0.350f, 0.750f, 1.000f, 1.000f, 1.000f, 0.900f, 0.500f},   // средний
            {0.000f, 0.500f, 0.750f, 0.900f, 1.000f, 1.000f, 0.750f, 0.500f, 0.250f, 0.000f, 0.000f},   // высокий
            {1.000f, 1.000f, 0.750f, 0.250f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f}   // максимальный
        };
        
        // Используется для оценки уровня пригодности учебной программыю
        float[,]  _meditationLevelMemberFunc = new float[MAX_LP,MAX_FPA] {
            {0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.550f, 0.800f, 1.000f},   // максимальный
            {0.000f, 0.000f, 0.000f, 0.000f, 0.350f, 0.750f, 1.000f, 1.000f, 1.000f, 0.900f, 0.500f},   // высокий
            {0.000f, 0.500f, 0.750f, 0.900f, 1.000f, 1.000f, 0.750f, 0.500f, 0.250f, 0.000f, 0.000f},   // средний
            {1.000f, 1.000f, 0.750f, 0.250f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f}    // низкий
        };
        
        // float[,]  _temperamentMemberFunc = new float[MAX_LP,MAX_FPA] {
        //     {0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f},   // сангвиник
        //     {0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f},   // флегматик
        //     {0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f},   // меланхолик
        //     {0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f}    // холерик
        // };

        public int GetOutTermDiscretenes()
        {
            return NUMBER_OF_POINT_OUT;
        }

        private float[,] _slideVelocityMemberFunc = new float[MAX_RULE_NUMBER, NUMBER_OF_POINT_OUT]
        {
            {
                0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, // очень быстро
                0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.700f, 1.000f, 1.000f, 1.000f, 1.000f
            },
            {
                0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, // быстро
                0.700f, 1.000f, 1.000f, 1.000f, 1.000f, 1.000f, 0.700f, 0.000f, 0.000f, 0.000f, 0.000f
            },
            {
                0.000f, 0.000f, 0.000f, 0.000f, 0.700f, 1.000f, 1.000f, 1.000f, 1.000f, 1.000f, // средне
                0.700f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f
            },
            {
                1.000f, 1.000f, 1.000f, 1.000f, 0.700f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, // медленно
                0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f, 0.000f
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
        
        public int FuzzyLogic(int age, int temperament, int attention)
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
                u[j] = MatrixDisjunction(_temperamentMemberFunc[j,temperament], _attentionLevelMemberFunc[j,attention]);
                u[j] = MatrixDisjunction(_ageMemberFunc[j,age], u[j]);
            }
            for (int j = 0; j < NUMBER_OF_POINT_OUT; j++)
            { 
                // вычисление выходной ф-ии принадлежности
                for (int i = 0; i < MAX_RULE_NUMBER; i++)
                { // вычисление выходов для каждого правила
                    itmp[i] = MatrixConjunction(u[i], _slideVelocityMemberFunc[i, j]); // модель импликации u[i] - антецедент,
                    tmp = MatrixDisjunction(_outputFuzzyFunction[j], itmp[i]); // композиция
                    _outputFuzzyFunction[j] = tmp;
                }
            }
            return CenterOfMass();
        }



    }
}