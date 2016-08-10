using Xamarin.Forms;

namespace XamarinForms.Droid.MyEntry
{
    public class MyEntry : Entry
    {
        //To jest ciekawe pozwala na tworzenie customowych typow z Xamarin.Form a te MyEntryRender na tworzenie zgodnie z platform¹

        public static readonly BindableProperty PointSizeProperty = BindableProperty.Create("PointSize", // propertyName 
            typeof(double), // returnType 
            typeof(MyEntry), // declaringType 
            8.0, // defaultValue 
            propertyChanged: OnPointSizeChanged);

        public MyEntry() {
            SetLabelFontSize((double)PointSizeProperty.DefaultValue);
        }

        public double PointSize {
            set { SetValue(PointSizeProperty, value); }
            get { return (double)GetValue(PointSizeProperty); }
        }

        static void OnPointSizeChanged(BindableObject bindable, object oldValue, object newValue) {
            ((MyEntry)bindable).OnPointSizeChanged((double)oldValue, (double)newValue);
        }

        void OnPointSizeChanged(double oldValue, double newValue) {
            SetLabelFontSize(newValue);
        }

        void SetLabelFontSize(double pointSize) { 
            FontSize = 160 * pointSize / 72;
        }

    }
}