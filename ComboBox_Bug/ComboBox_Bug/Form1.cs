using System;
using System.ComponentModel;
using System.Windows.Forms;

/* Run App -> no events are fired, dispite the BindingList getting changed after all 
 *		Bindings are setup
 * Select Item from combobox manually -> only comboBox1_SelectedIndexChanged gets called,
 *		the databound string Bar doesnt get set
 * Close App -> the databound string Bar changes
 * 
 * uncomment comboBox1.DataBindings[nameof(comboBox1.SelectedItem)].WriteValue(); in
 *		private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
 * Run App -> like before no breakpoints are reached
 * Select Item from combobox manually -> only comboBox1_SelectedIndexChanged gets called,
 *		the databound string Bar gets set through the workaround in
 *		private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
 * Close App -> the databound string Bar gets set again, no events called
 */

namespace ComboBox_Bug
{
	public partial class Form1 : Form, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public BindingList<string> Foo { get; } = new BindingList<string>();

		string bar;
		public string Bar
		{
			get => bar;
			set
			{
				// only gets called on closing the app
				// does not get called after User selected the "test" item in the combobox
				bar = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Bar)));
			}
		}

		public Form1()
		{
			InitializeComponent();
			comboBox1.DataSource = Foo;
			comboBox1.DataBindings.Add(nameof(comboBox1.SelectedItem), this, nameof(Bar),
				false, DataSourceUpdateMode.OnPropertyChanged);
			// SelectedIndex == -1
			Foo.Add("test");
			// SelectedIndex == 0
			// No events fired
			// No associated DataBindings are taken care of
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			// only gets called after User selected the "test" item in the combobox
		}

		/* workaround because Bar doesnt get set when User selects the "test" item
		* in the combobox
		* all actions that fire SelectedIndexChanged should update Bar
		* all actions that change the SelectedIndex should fire SelectedIndexChanged
		*/
		private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
		{
			// comboBox1.DataBindings[nameof(comboBox1.SelectedItem)].WriteValue();
		}
	}
}
