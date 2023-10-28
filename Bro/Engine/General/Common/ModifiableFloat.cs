using System.Collections.Generic;

namespace Bro
{
    [System.Serializable]
    public class ModifiableFloat
    {
        protected float _baseValue;

        public float BaseValue
        {
            get { return _baseValue; }
            set
            {
                _baseValue = value;
                Recalculate();
            }
        }

        protected float _value;

        [Bro.Json.JsonIgnoreAttribute]
        public float Value
        {
            get { return _value; }
        }

        public ModifiableFloat() : this(0f)
        {
        }

        public ModifiableFloat(float baseValue)
        {
            BaseValue = baseValue;
        }

        public void Reset()
        {
            _multipliers.Clear();
            _adders.Clear();
            Recalculate();
        }


        private Dictionary<int, float> _multipliers = new Dictionary<int, float>();

        public void SetMultiplier(int key, float mult)
        {
            _multipliers[key] = mult;
            Recalculate();
        }

        public void RemoveMultiplier(int key)
        {
            if (_multipliers.Remove(key))
            {
                Recalculate();
            }
            else
            {
                Bro.Log.Error("Cannot find multiplier ");
            }
        }

        [Bro.Json.JsonIgnoreAttribute]
        public float TotalMultiplier
        {
            get
            {
                float result = 1f;
                foreach (var m in _multipliers)
                {
                    result *= m.Value;
                }

                return result;
            }
        }


        private Dictionary<int, float> _adders = new Dictionary<int, float>();

        public void SetAdder(int key, float adder)
        {
            _adders[key] = adder;
            Recalculate();
        }


        public void RemoveAdder(int key)
        {
            if (_adders.Remove(key))
            {
                Recalculate();
            }
            else
            {
                Bro.Log.Error("Cannot find Adder");
            }
        }

        [Bro.Json.JsonIgnoreAttribute]
        public float TotalAdder
        {
            get
            {
                float result = 0f;
                foreach (var a in _adders)
                {
                    result += a.Value;
                }

                return result;
            }
        }


        protected virtual void Recalculate()
        {
            _value = (BaseValue + TotalAdder) * TotalMultiplier;
        }


        [System.Serializable]
        public class WithInvAndSqr : ModifiableFloat
        {
            private float _sqrValue;

            [Bro.Json.JsonIgnoreAttribute]
            public float SqrValue
            {
                get { return _sqrValue; }
            }

            private float _invValue;

            [Bro.Json.JsonIgnoreAttribute]
            public float InvValue
            {
                get { return _invValue; }
            }

            protected override void Recalculate()
            {
                _value = (BaseValue + TotalAdder) * TotalMultiplier;
                _invValue = 1f / _value;
                _sqrValue = _value * _value;
            }

            public WithInvAndSqr()
                : base()
            {
            }

            public WithInvAndSqr(float baseValue)
                : base(baseValue)
            {
            }
        }
    }
}