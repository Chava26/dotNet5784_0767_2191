//using System.Windows;
//using System.Windows.Controls;
//using Microsoft.Xaml.Behaviors;

//namespace PL.Behaviors
//{
//    public class PasswordBehavior : Behavior<PasswordBox>
//    {
//        public static readonly DependencyProperty PasswordProperty =
//            DependencyProperty.Register(nameof(Password), typeof(string), typeof(PasswordBehavior),
//                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

//        public string Password
//        {
//            get => (string)GetValue(PasswordProperty);
//            set => SetValue(PasswordProperty, value);
//        }

//        protected override void OnAttached()
//        {
//            base.OnAttached();
//            AssociatedObject.PasswordChanged += OnPasswordChanged;
//        }

//        protected override void OnDetaching()
//        {
//            base.OnDetaching();
//            AssociatedObject.PasswordChanged -= OnPasswordChanged;
//        }

//        private void OnPasswordChanged(object sender, RoutedEventArgs e)
//        {
//            Password = AssociatedObject.Password;
//        }
//    }
//}
