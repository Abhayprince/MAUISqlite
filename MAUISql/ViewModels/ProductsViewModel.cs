using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUISql.Data;
using MAUISql.Models;
using System.Collections.ObjectModel;

namespace MAUISql.ViewModels
{
    public partial class ProductsViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        public ProductsViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private ObservableCollection<Product> _products;

        [ObservableProperty]
        private Product _operatingProduct = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _busyText;

        [RelayCommand]
        private async Task LoadProductsAsync()
        {
            await ExecuteAsync(async () =>
            {
                var products = await _context.GetAllAsync<Product>();
                if (products is not null && products.Any())
                {
                    Products ??= new ObservableCollection<Product>();

                    foreach (var product in products)
                    {
                        Products.Add(product);
                    }
                }
            }, "Fetching products...");
        }

        [RelayCommand]
        private void SetOperatingProduct(Product? product) => OperatingProduct = product ?? new();

        [RelayCommand]
        private async Task SaveProductAsync()
        {
            if (OperatingProduct is null)
                return;
            var busyText = OperatingProduct.Id == 0 ? "Creating product..." : "Updating product";
            await ExecuteAsync(async () =>
            {
                if (OperatingProduct.Id == 0)
                {
                    // Create product
                    await _context.AddItemAsync<Product>(OperatingProduct);
                    Products.Add(OperatingProduct);
                }
                else
                {
                    // Update product
                    await _context.UpdateItemAsync<Product>(OperatingProduct);

                    var productCopy = OperatingProduct.Clone();

                    var index = Products.IndexOf(OperatingProduct);
                    Products.RemoveAt(index);

                    Products.Insert(index, productCopy);
                }
                SetOperatingProductCommand.Execute(new());
            }, busyText);
        }

        [RelayCommand]
        private async Task DeleteProductAsync(int id)
        {
            await ExecuteAsync(async () =>
            {
                if (await _context.DeleteItemByKeyAsync<Product>(id))
                {
                    var product = Products.FirstOrDefault(p => p.Id == id);
                    Products.Remove(product);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Delete Error", "Product was not deleted", "Ok");
                }
            }, "Deleting product...");
        }

        private async Task ExecuteAsync(Func<Task> operation, string? busyText = null)
        {
            IsBusy = true;
            BusyText = busyText ?? "Processing...";
            try
            {
                await operation?.Invoke();
            }
            finally
            {
                IsBusy = false;
                BusyText = "Processing...";
            }
        }
    }
}
