using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ServerWPF
{
    class TextBoxOutput
    {
        private TextBox textBox;

        public TextBoxOutput(TextBox textBox)
        {
            this.textBox = textBox;
        }

        public void Add(string text)
        {
            this.textBox.AppendText(text + "\n");
        }
        
    }
}
