//-----------------------------------------------------------------------
// <copyright file="MtListPicker.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections;
using Microsoft.Phone.Controls;

namespace MyToolkit.Controls
{
	public class MtListPicker : ListPicker
	{
		//public static readonly DependencyProperty MySelectedItemsProperty =
		//    DependencyProperty.Register("TypedSelectedItems", typeof(IList), typeof(MtListPicker),
		//    new PropertyMetadata(default(IEnumerable), PropertyChangedCallback));

		//private static void PropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		//{
		//    var ctrl = (MtListPicker)obj;
		//    ctrl.Update();
		//}

		//private bool bindingRegistered = false;
		//private void Update()
		//{
		//    if (!bindingRegistered)
		//    {
		//        var binding = new Binding("SelectedItems");
		//        binding.Source = new Helper(this);
		//        binding.Mode = BindingMode.TwoWay;
		//        SetBinding(SelectedItemsProperty, binding);
		//        bindingRegistered = true;
		//    }

		//    SelectedItems = TypedSelectedItems;
		//}

		//protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		//{
		//    if (SelectedItems != null)
		//    {
		//        var list = SelectedItems.Cast<object>().
		//            Where(item => !Items.Contains(item)).ToList();
		//        foreach (var item in list)
		//            SelectedItems.Remove(item);
		//    }

		//    if (!Items.Contains(SelectedItem))
		//        SelectedIndex = -1; 
		//        //SelectedItem = null; 

		//    base.OnItemsChanged(e);
		//}

		//public class Helper
		//{
		//    private readonly MtListPicker picker;
		//    public Helper(MtListPicker p)
		//    {
		//        picker = p;
		//    }

		//    public IList SelectedItems
		//    {
		//        get { return picker.TypedSelectedItems; }
		//        set
		//        {
		//            if (picker.TypedSelectedItems != null && value != null)
		//            {
		//                var elementType = picker.TypedSelectedItems.GetType().GetGenericArguments()[0];
		//                var type = value.GetType().GetGenericTypeDefinition().MakeGenericType(elementType);
		//                if (value.GetType() != type)
		//                {
		//                    var method = typeof(Enumerable).GetMethod("OfType").MakeGenericMethod(elementType);
		//                    var list = method.Invoke(null, new object[] { value });
		//                    picker.TypedSelectedItems = (IList)Activator.CreateInstance(type, list);
		//                }
		//                else
		//                    picker.TypedSelectedItems = value;
		//            }
		//            else
		//                picker.TypedSelectedItems = value;
		//        }
		//    }
		//}

		//public IList TypedSelectedItems
		//{
		//    get { return (IList)GetValue(MySelectedItemsProperty); }
		//    set { SetValue(MySelectedItemsProperty, value); }
		//}

		// used to make setter method public
		public new IList SelectedItems
		{
			get { return (IList)GetValue(SelectedItemsProperty); }
			set { base.SetValue(SelectedItemsProperty, value); }
		}
	}
}