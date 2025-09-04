using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymTracker.Services
{
    public class WeightInputBehaviour : Behavior<Entry>
    {
        public int MaxDecimalPlaces { get; set; } = 2;

        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnTextChanged;
            base.OnDetachingFrom(entry);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = (Entry)sender;

            if (string.IsNullOrWhiteSpace(entry.Text))
                return;

            if (decimal.TryParse(entry.Text, out var value))
            {
                var formatted = value.ToString($"0.{new string('#', MaxDecimalPlaces)}");

                if (entry.Text != formatted)
                    entry.Text = formatted;
            }
            else
            {
                entry.Text = string.Empty;
            }
        }
    }

}
