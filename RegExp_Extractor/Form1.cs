using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RegExp_Extractor
{
	public partial class Form1 : Form
	{

		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{

			if (textBox1.Text.Length == 0)
			{
				MessageBox.Show("please input regular pattern");
				return;
			}

			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				listView1.Columns.Clear();
				listView1.Items.Clear();
				bool first = true;
				foreach (GroupCollection item in this.match(openFileDialog1.FileName, textBox1.Text))
				{
					var items = item.Cast<Group>().Select(x => x.ToString());

					if (first)
					{
						first = false;
						var headers = items.Select(x =>
						{
							var c = new ColumnHeader();
							c.Tag = true;
							return c;
						});
						listView1.Columns.AddRange(headers.ToArray());
					}

					listView1.Items.Add(new ListViewItem(items.ToArray()));
				}
				button2.Enabled = listView1.Items.Count > 0;
			}
		}

		private IEnumerable<GroupCollection> match(string file, string reg)
		{
			using (var sr = new StreamReader(openFileDialog1.FileName))
			{
				while (!sr.EndOfStream)
				{
					var line = sr.ReadLine();
					var match = Regex.Match(line, textBox1.Text);
					if (match.Success)
					{
						yield return match.Groups;
					}
				}
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				using (var sw = new StreamWriter(saveFileDialog1.FileName, false, Encoding.GetEncoding("Shift_JIS")))
				{
					int index = 0;
					bool[] include = new bool[listView1.Columns.Count];
					foreach (ColumnHeader item in listView1.Columns)
					{
						include[index++] = Convert.ToBoolean(item.Tag);
					}
					foreach (ListViewItem item in listView1.Items)
					{
						index = 0;
						var data = new List<string>();
						foreach (ListViewItem.ListViewSubItem i in item.SubItems)
						{
							if (index < include.Length && include[index])
							{
								data.Add(i.Text);
							}
							index++;
						}
						sw.Write(string.Join(",", data));
						sw.Write("\r\n");
					}
				}
			}

		}

		private void listView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
		{
			e.DrawBackground();
			bool value = Convert.ToBoolean(e.Header.Tag);
			CheckBoxRenderer.DrawCheckBox(e.Graphics,
				new Point(e.Bounds.Left + 4, e.Bounds.Top + 4),
				value ? System.Windows.Forms.VisualStyles.CheckBoxState.CheckedNormal :
				System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal);
		}

		private void listView1_DrawItem(object sender, DrawListViewItemEventArgs e)
		{
			e.DrawDefault = true;
		}

		private void listView1_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
		{
			e.DrawDefault = true;
		}

		private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			bool value = Convert.ToBoolean(this.listView1.Columns[e.Column].Tag);
			this.listView1.Columns[e.Column].Tag = !value;
			this.listView1.Invalidate();
		}
	}
}
