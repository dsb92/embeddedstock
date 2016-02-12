/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:PCApplikationMVVM"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

// Toolkit namespace
using PCApplikation;
using PCApplikationMVVM.ViewModels;

namespace PCApplikationMVVM
{
    /// <summary>
    /// This class creates ViewModels on demand for Views, supplying a
    /// ServiceAgent to the ViewModel if required.
    /// <para>
    /// Place the ViewModelLocator in the App.xaml resources:
    /// </para>
    /// <code>
    /// &lt;Application.Resources&gt;
    ///     &lt;vm:ViewModelLocator xmlns:vm="clr-namespace:PCApplikationMVVM"
    ///                                  x:Key="Locator" /&gt;
    /// &lt;/Application.Resources&gt;
    /// </code>
    /// <para>
    /// Then use:
    /// </para>
    /// <code>
    /// DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
    /// </code>
    /// <para>
    /// Use the <strong>mvvmlocator</strong> or <strong>mvvmlocatornosa</strong>
    /// code snippets to add ViewModels to this locator.
    /// </para>
    /// </summary>

    //// Create CustomerViewModel on demand
    //public ComponentsViewModel ComponentsViewModel
    //{
    //    get { return new ComponentsViewModel(); }
    //}

    public class ViewModelLocator
    {
        private MainViewModel mainViewModel_;
        public MainViewModel MainViewModel
        {
            get
            {
                if (mainViewModel_ == null)
                {
                    mainViewModel_ = new MainViewModel();
                }
                return mainViewModel_;
            }
        }

        private CreateComponentViewModel createComponentViewModel_;
        public CreateComponentViewModel CreateComponentViewModel
        {
            get
            {
                if (createComponentViewModel_ == null)
                {
                    createComponentViewModel_ = new CreateComponentViewModel(new Component());
                }
                return createComponentViewModel_;
            }
            set { createComponentViewModel_ = value; }
        }


        private ChangeComponentViewModel changeComponentViewModel_;
        public ChangeComponentViewModel NewChangeComponentViewModel
        {
            get
            {
                if (changeComponentViewModel_ == null)
                {
                    changeComponentViewModel_ = new ChangeComponentViewModel(new Component());
                }
                return changeComponentViewModel_;
            }
            set { changeComponentViewModel_ = value; }
        }

        private LogInViewModel logInViewModel_; //Har måske ikke brug for dette
        public LogInViewModel NewLogInViewModel
        {
            get
            {
                if (logInViewModel_ == null)
                {
                    logInViewModel_= new LogInViewModel();
                }
                return logInViewModel_;
            }
        }
        private LoanViewModel loansViewModel_; //Har måske ikke brug for dette
        public LoanViewModel LoansViewModel
        {
            get
            {
                return loansViewModel_;
            }
            set { loansViewModel_ = value; }
        }
    }
}