using MAUISql.ViewModels;

namespace MAUISql;

public partial class MainPage : ContentPage
{
    private readonly ProductsViewModel _viewModel;

    public MainPage(ProductsViewModel viewModel)
	{
		InitializeComponent();
        BindingContext= viewModel;
        _viewModel = viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadProductsAsync();
    }
}

