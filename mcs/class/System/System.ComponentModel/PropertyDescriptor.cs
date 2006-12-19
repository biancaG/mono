//
// System.ComponentModel.PropertyDescriptor.cs
//
// Author:
//  Miguel de Icaza (miguel@ximian.com)
//  Andreas Nahr (ClassDevelopment@A-SoftTech.com)
//
// (C) Ximian, Inc.  http://www.ximian.com
// (C) 2003 Andreas Nahr
//

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.ComponentModel
{
	[ComVisible (true)]
	public abstract class PropertyDescriptor : MemberDescriptor
	{
		TypeConverter converter;

		protected PropertyDescriptor (MemberDescriptor reference)
		: base (reference)
		{
		}

		protected PropertyDescriptor (MemberDescriptor reference, Attribute [] attrs)
		: base (reference, attrs)
		{
		}

		protected PropertyDescriptor (string name, Attribute [] attrs)
		: base (name, attrs)
		{
		}

		public abstract Type ComponentType { get; }

		public virtual TypeConverter Converter {
			get {
				if (converter == null) {
					TypeConverterAttribute at = (TypeConverterAttribute) Attributes [typeof(TypeConverterAttribute)];
					if (at == null || at == TypeConverterAttribute.Default)
						converter = TypeDescriptor.GetConverter (PropertyType);
					else {
						Type t = Type.GetType (at.ConverterTypeName);
						if (t == null) {
							converter = TypeDescriptor.GetConverter (PropertyType);
						}
						else {
							ConstructorInfo ci = t.GetConstructor (new Type[] { typeof(Type) });
							if (ci != null)
								converter = (TypeConverter) ci.Invoke (new object[] { PropertyType });
							else
								converter = (TypeConverter) Activator.CreateInstance (t);
						}
					}
				}
				return converter;
			}
		}

		public virtual bool IsLocalizable {
			get {
				foreach (Attribute attr in AttributeArray){
					if (attr is LocalizableAttribute)
						return ((LocalizableAttribute) attr).IsLocalizable;
				}

				return false;
			}
		}

		public abstract bool IsReadOnly { get; }

		public abstract Type PropertyType { get; }

		public DesignerSerializationVisibility SerializationVisibility {
			get {
				foreach (Attribute attr in AttributeArray) {
					if (attr is DesignerSerializationVisibilityAttribute){
						DesignerSerializationVisibilityAttribute a;

						a = (DesignerSerializationVisibilityAttribute) attr;

						return a.Visibility;
					}
				}

				return DesignerSerializationVisibility.Visible;
			}
		}

		Hashtable notifiers;

		public virtual void AddValueChanged (object component, EventHandler handler)
		{
			EventHandler component_notifiers;

			if (component == null)
				throw new ArgumentNullException ("component");

			if (handler == null)
				throw new ArgumentNullException ("handler");

			if (notifiers == null)
				notifiers = new Hashtable ();

			component_notifiers = (EventHandler) notifiers [component];

			if (component_notifiers != null)
				component_notifiers += handler;
			else
				notifiers [component] = handler;
		}

		public virtual void RemoveValueChanged (object component, System.EventHandler handler)
		{
			EventHandler component_notifiers;

			if (component == null)
				throw new ArgumentNullException ("component");

			if (handler == null)
				throw new ArgumentNullException ("handler");

			if (notifiers == null) return;

			component_notifiers = (EventHandler) notifiers [component];
			component_notifiers -= handler;
			
			if (component_notifiers == null)
				notifiers.Remove (component);
			else
				notifiers [component] = handler;
		}

		protected virtual void OnValueChanged (object component, EventArgs e)
		{
			if (notifiers == null)
				return;

			EventHandler component_notifiers = (EventHandler) notifiers [component];

			if (component_notifiers == null)
				return;

			component_notifiers (component, e);
		}

		public abstract object GetValue (object component);

		public abstract void SetValue (object component, object value);

		public abstract void ResetValue (object component);

		public abstract bool CanResetValue (object component);

		public abstract bool ShouldSerializeValue (object component);

		protected object CreateInstance(System.Type type)
		{
			return Assembly.GetExecutingAssembly ().CreateInstance (type.Name);
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals (obj)) return false;
			PropertyDescriptor other = obj as PropertyDescriptor;
			if (other == null) return false;
			return other.PropertyType == PropertyType;
		}

		public PropertyDescriptorCollection GetChildProperties()
		{
			return GetChildProperties (null, null);
		}

		public PropertyDescriptorCollection GetChildProperties(object instance)
		{
			return GetChildProperties (instance, null);
		}

		public PropertyDescriptorCollection GetChildProperties(Attribute[] filter)
		{
			return GetChildProperties (null, filter);
		}

		public override int GetHashCode() 
		{
			return base.GetHashCode ();
		}

		public virtual PropertyDescriptorCollection GetChildProperties (object instance, Attribute[] filter)
		{
			return TypeDescriptor.GetProperties (instance, filter);
		}

		public virtual object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor (PropertyType, editorBaseType);
		}

		protected Type GetTypeFromName(string typeName)
		{
			return Type.GetType (typeName);
		}
	}
}

