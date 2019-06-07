using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Threads
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Test();
        }

        private async void Test()
        {
            var thread = Thread.CurrentThread.ManagedThreadId;

            await Task.Run(async () =>
            {
                thread = Thread.CurrentThread.ManagedThreadId;
            });

            thread = Thread.CurrentThread.ManagedThreadId;

        }
    }
}
