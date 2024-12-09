using System.ComponentModel;
using ViewModel;

namespace TestViewModel
{
    class FakeVM : BaseViewModel
    {

        private string _testProperty;

        public string TestProperty
        {
            get => _testProperty; 
            set => SetProperty(ref _testProperty, value);
        }

        public FakeVM()
        {
            _testProperty = "Property unchanged";
        }

        public void TriggerPropertyChanged()
        {
            OnPropertyChanged("Test");
        }
    }

    public class TestBaseVM
    {
        private string? lastPropertyChanged;

        [Fact]
        public void TestOnPropertyChanged()
        {
            FakeVM vm = new FakeVM();
            vm.PropertyChanged += VM_OnPropertyChanged;
            vm.TriggerPropertyChanged();
            Assert.Equal("Test", lastPropertyChanged);
        }

        [Fact]
        public void TestSetProperty()
        {
            var vm = new FakeVM();

            bool isPropertyChanged = false;

            vm.PropertyChanged += (sender, args) =>
            {
                isPropertyChanged = true;
            };

            vm.TestProperty = "Property Changed";

            Assert.True(isPropertyChanged, "Property Changed was not triggered for TestProperty");
        }

        private void VM_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            lastPropertyChanged = e.PropertyName;
        }
    }
}