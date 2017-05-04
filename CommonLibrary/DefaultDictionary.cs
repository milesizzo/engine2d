using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public class DefaultDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        private readonly Func<TKey, TValue> generator;

        public DefaultDictionary(Func<TKey, TValue> generator)
        {
            this.generator = generator;
        }

        public new TValue this[TKey key]
        {
            get
            {
                TValue value;
                if (!this.TryGetValue(key, out value))
                {
                    value = this.generator(key);
                    this.Add(key, value);
                }
                return value;
            }
            set { base[key] = value; }
        }
    }
}
