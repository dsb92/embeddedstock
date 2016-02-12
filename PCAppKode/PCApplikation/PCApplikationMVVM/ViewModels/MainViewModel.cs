using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Security.RightsManagement;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media.Imaging;
using DataAccesLogicLib;
using MoreLinq;
using PCApplikationMVVM.Views;
using SimpleMvvmToolkit;



namespace PCApplikationMVVM.ViewModels
{
	public class MainViewModel : ViewModelBase<MainViewModel>
	{
		private Component newComponent_;
		private ComponentDataUtil cdbUtil_;
		private LoanDataUtil ldbUtil_;
		private Thread loanInformationUpdaterThread_;

		public MainViewModel()
		{
			//Initialization:
			selectedItemList_ = new ObservableCollection<Component>();
			componentList_ = new ObservableCollection<Component>();
			allComponentList_ = new ObservableCollection<Component>();
			emailList_ = new ObservableCollection<Component>();
			cdbUtil_ = new ComponentDataUtil();
			ldbUtil_ = new LoanDataUtil();

			//Get all components from database
			Refresh();

			//Reset PW
			//Properties.Settings.Default.Reset();
		}

		#region Properties

		//Property used by loading bar, the maximum of componenets to be processed
		private int maxLoadbar_;
		public int MaxLoadbar
		{
			get { return maxLoadbar_; }
			set
			{
				maxLoadbar_ = value;
				NotifyPropertyChanged(m => m.MaxLoadbar);
			}
		}

		//Used by loading bar, the amount processed
		private int loadbarProgress_;
		public int LoadbarProgress
		{
			get { return loadbarProgress_; }
			set
			{
				loadbarProgress_ = value;
				NotifyPropertyChanged(m => m.LoadbarProgress);
			}
		}

		//Menu text, displays loading bar status
		private string statusBarText_;
		public string StatusBarText
		{
			get { return statusBarText_; }
			set
			{
				statusBarText_ = value;
				NotifyPropertyChanged(m => m.StatusBarText);
			}
		}

		//Controls loading bar visibility
		private Visibility loadBarVisibility_;
		public Visibility LoadBarVisibility
		{
			get { return loadBarVisibility_; }
			set
			{
				loadBarVisibility_ = value;
				NotifyPropertyChanged(m => m.LoadBarVisibility);
			}
		}

		//Keeps track on whether we're logged in
		private bool _loggedIn = true;
		public bool LoggedIn
		{
			get { return _loggedIn; }
			set
			{
				_loggedIn = value;
				NotifyPropertyChanged(m => m.LoggedIn);
			}
		}

		//Keeps track on wheter we're logged in (these two are needed for some databinding stuff)
		private bool _loggedout = true;
		public bool LoggedOut
		{
			get { return _loggedout; }
			set
			{
				_loggedout = value;
				NotifyPropertyChanged(m => m.LoggedOut);
			}
		}

		//Todo ask martin wtf
		private string _secondDataGrid;
		public string SecondDataGrid
		{
			get { return _secondDataGrid; }
			set
			{
				_secondDataGrid = value;
				NotifyPropertyChanged(m => m.SecondDataGrid);
			}
		}

		private string _firstDataGrid;
		public string FirstDataGrid
		{
			get { return _firstDataGrid; }
			set
			{
				_firstDataGrid = value;
				NotifyPropertyChanged(m => m.FirstDataGrid);
			}
		}

		//Search text
		private string _searchText = "Skriv her for at søge";
		public string SearchText
		{
			get { return _searchText; }
			set
			{
				_searchText = value;
				NotifyPropertyChanged(m => m.SearchText);
			}
		}

		//List of components currently displayed in the application
		private ObservableCollection<Component> componentList_;
		public ObservableCollection<Component> ComponentList
		{
			get { return componentList_; }
			set { componentList_ = value; }
		}

		//List of all the componenets last updated form the Database
		private ObservableCollection<Component> allComponentList_;
		public ObservableCollection<Component> AllComponentList
		{
			get { return allComponentList_; }
			set { allComponentList_ = value; }
		}

		//List of components where user needs to get an email
		private ObservableCollection<Component> emailList_;
		public ObservableCollection<Component> EmailList
		{
			get { return emailList_; }
			set { emailList_ = value; }
		}

		//The currently selected item
		private Component selectedItem_;
		public Component SelectedItem
		{
			get { return selectedItem_; }
			set
			{
				selectedItem_ = value;
				NotifyPropertyChanged(m => m.SelectedItem);
			}
		}

		//If more items are selected, these are added to this list via Event in Main View
		private IList<Component> selectedItemList_;
		public IList<Component> SelectedItemList
		{
			get { return selectedItemList_; }
			private set
			{
				selectedItemList_ = value;
				NotifyPropertyChanged(m => m.selectedItemList_);
			}
		}

		//Keeps track on wheter we're showing unique or non-unique componenets
		private bool _uniqeChecked;
		public bool UniqeChecked
		{
			get { return _uniqeChecked; }
			set
			{
				//Refresh(); //maybe needed
				_uniqeChecked = value;
				NotifyPropertyChanged(m => m.UniqeChecked);
				if (CategoryIndex == -1)
				{
					CategoryIndex = 0;
				}
				else
				{
					int tempcategoryindex = CategoryIndex;
					CategoryIndex = -1;
					CategoryIndex = tempcategoryindex;
				}

			}
		}

		//Keeps track on which index category we're currently on //todo ask Martin to give a short explanation
		private int _categoryIndex;
		public int CategoryIndex
		{
			get { return _categoryIndex; }
			set
			{
				_categoryIndex = value;
				NotifyPropertyChanged(m => m.CategoryIndex);
			}
		}

		//Keeps track on which category name we're currently on //todo ask Martin to give a short explanation
		private string selectedCategory_;
		public string SelectedCategory
		{
			get { return selectedCategory_; }
			set
			{
				selectedCategory_ = value;

				if (CategoryIndex != -1)
				{
					SetComponentList(new ObservableCollection<Component>(GetComponentsFromCategory(selectedCategory_)));
				}

			}
		}

		//Easy way to get application ressource categories
		public Categories Categories
		{
			get { return (Categories)Application.Current.FindResource("Categories"); }
		}

		#endregion

		#region Methods

		//Function that helps adding selected items to a list of selected components, used by Main view code behind.
		public void AddToSelectedItemsList(IList components)
		{
			SelectedItemList.Clear();

			foreach (Component c in components)
			{
				SelectedItemList.Add(c);
			}
		}

		//Sets the current Component list we're watching in the main view
		//This function is my best work ever Martin Nielsen Fig
		public void SetComponentList(ObservableCollection<Component> list)
		{
			ComponentList.Clear();
			if (true == UniqeChecked)
			{
				ObservableCollection<Component> distictlist;
				distictlist = new ObservableCollection<Component>(list.DistinctBy(x => x.ComponentName));

				foreach (var component in distictlist)
				{
					ComponentList.Add(component);
				}
				FirstDataGrid = "Visible";
				SecondDataGrid = "Collapsed";
				ComponetAmountValue();
				LoansAmountValue();
			}
			else
			{
				foreach (var component in list)
				{
					ComponentList.Add(component);
				}
				FirstDataGrid = "Collapsed";
				SecondDataGrid = "Visible";
			}
		}

		//Gets the next available component number depending on selected item. Used when creating a component
		public int GetNextComponentNumber(string name)
		{
			int res = 0;
			if (string.IsNullOrEmpty(name))
			{
				foreach (Component comp in AllComponentList)
				{
					if (res < comp.ComponentNumber)
						res = comp.ComponentNumber;
				}
				return ++res;
			}
			else
			{
				foreach (Component comp in AllComponentList)
				{
					if (comp.ComponentName == name)
					{
						if (res < comp.ComponentNumber)
							res = comp.ComponentNumber;
					}
				}
				return ++res;
			}
		}

		//Builds the application wide Categories, get's each distinct category by looking at all the components
		private void GetUniqueCategories()
		{
			var cat = new List<string>();

			foreach (Component c in allComponentList_)
			{
				cat.Add(c.Category);
			}

			//Filter: get only distinct categories
			var result = cat.Select(e => e.ToString()).Distinct().ToList();
			result.Sort();
			var categories = (ObservableCollection<string>)Application.Current.FindResource("Categories");

			foreach (string s in result)
			{
				categories.Add(s);
			}
		}

		//Gets all componenets from a certain category
		private List<Component> GetComponentsFromCategory(string name)
		{
			List<Component> temp = new List<Component>();

			if (name == "Alle")
			{
				foreach (var component in AllComponentList)
				{
					temp.Add(component);
				}
			}
			else
			{
				foreach (var component in AllComponentList)
				{
					if (name == component.Category)
					{
						temp.Add(component);
					}

				}
			}
			return temp;
		}

		//Logs out
		public void LogOut()
		{
			LoggedIn = false;
			LoggedOut = true;
		}

		//Logs in
		public void LogIn()
		{
			LoggedOut = false;
			LoggedIn = true;
		}

		//Locks the application / greys it out, open by calling log in
		public void LockApplication()
		{
			LoggedIn = false;
			LoggedOut = false;
		}

		//Refreshes the entire component catalogue by removing local, and then quering the database for them all again, also rebuilds categories
		public void Refresh()
		{
			try
			{
                //Check if any reservation is older than 5 days, then deletes them
                cdbUtil_.CheckReservations();
                
				AllComponentList = new ObservableCollection<Component>(cdbUtil_.GetAllComponents());
				var allLoanInformationList = new ObservableCollection<LoanInformation>(ldbUtil_.GetAllLoanInformation());

				loanInformationUpdaterThread_ = new Thread(() =>
				{
					//Set up load bar:
					MaxLoadbar = AllComponentList.Count;
					StatusBarText = "Henter låne information...";
					LoadBarVisibility = Visibility.Visible;

					foreach (LoanInformation li in allLoanInformationList)
					{
						foreach (Component c in AllComponentList)
						{
							if (li.Component == c.ComponentID)
							{
								c.ActualLoanInformation = li;
								LoadbarProgress++;
								break;
							}
						}
					}

					//Clean load bar:
					StatusBarText = "Klar";
					LoadbarProgress = 0;
					LoadBarVisibility = Visibility.Hidden;
				});

				loanInformationUpdaterThread_.Start();
			}
			catch
			{
				Reconnect();
				AllComponentList = new ObservableCollection<Component>();
			}
			Categories.Clear();
			Categories.Add("Alle");
			GetUniqueCategories();
			SetComponentList(AllComponentList);
			//SendExpLoanEmail();
		}

		//Search through all components, using search algorithm in the DAL layer
		public void Search()
		{
			WaitForLoanInformationUpdated();

			char[] seperators = { ' ' };
			List<string> tempList = new List<string>();
			tempList = SearchText.Split(seperators, StringSplitOptions.RemoveEmptyEntries).ToList();

			var searchResults = cdbUtil_.GetComponents(tempList);
			SetComponentList(new ObservableCollection<Component>(searchResults));

			loanInformationUpdaterThread_ = new Thread(() =>
			{
				//Set up load bar:
				MaxLoadbar = searchResults.Count;
				StatusBarText = "Søger ...";
				LoadBarVisibility = Visibility.Visible;

				foreach (LoanInformation li in ldbUtil_.GetAllLoanInformation())
				{
					foreach (Component c in searchResults)
					{
						if (li.Component == c.ComponentID)
						{
							c.ActualLoanInformation = li;
							LoadbarProgress++;
							break;
						}
					}
				}

				//Parallel version, not sure if safe
				//Parallel.ForEach(allLoanInformationList, li =>
				//{
				//	foreach (Component c in AllComponentList)
				//	{
				//		if (li.Component == c.ComponentID)
				//		{
				//			c.ActualLoanInformation = li;
				//			Interlocked.Increment(ref loadbarProgress_);
				//			break;
				//		}
				//	}
				//});

				//Clean load bar:
				StatusBarText = "Klar";
				LoadbarProgress = 0;
				LoadBarVisibility = Visibility.Hidden;
			});
			loanInformationUpdaterThread_.Start();

			CategoryIndex = -1; //Makes sure that nothing is selected in category list
		}

		//Informs user about problems with server, then tries to reconnect
		public void Reconnect()
		{
			var result = MessageBox.Show("Ingen forbindelse til databasen, vil du prøve at genstarte?",
				"Forbindelses Fejl", MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes)
			{
				System.Windows.Forms.Application.Restart();
				System.Windows.Application.Current.Shutdown();
			}
		}

		//Counts the number of the same component
		public void ComponetAmountValue()
		{
			foreach (var component in ComponentList)
			{
				int amount = 0;
				foreach (var component1 in AllComponentList)
				{
					if (component1.ComponentName == component.ComponentName)
					{
						amount++;
					}
				}
				component.AmountComponets = amount;
			}
		}
		public void LoansAmountValue() //TODO IMPLEMENT
		{
			foreach (var component in ComponentList)
			{
				int amount = 0;
				foreach (var component1 in AllComponentList)
				{
					if (component1.ComponentName == component.ComponentName && !string.IsNullOrEmpty(component1.ActualLoanInformation.OwnerID))
					{
						amount++;
					}
				}
				component.AmountLoaned = amount;

			}
		}
		//Send email to all people who has an component with expired loan date, but only once (no more than 1 email pr. user pr. component)

		public void SendExpLoanEmail()
		{
			//DateTime today = DateTime.Today;
			//SqlDateTime? today = DateTime.Today;
			foreach (Component component in AllComponentList)
			{
				if (component.ActualLoanInformation.ReturnDate != null)
					if ((component.ActualLoanInformation.ReturnDate.Value < DateTime.Now) && (component.ActualLoanInformation.IsEmailSend == "yes"))
					{
						EmailList.Add(component);
					}
			}
			
            foreach (Component component in EmailList)
			{
				try
				{
					MailMessage mail = new MailMessage();
					//put your SMTP address and port here.
					SmtpClient smtpServer = new SmtpClient("smtp.", 587); //todo skal rettes til med mail og port for skolens udbyders SMPT server.
					//Set the secure socket layer to true.
					smtpServer.EnableSsl = true;
					//Your username and password!
					smtpServer.Credentials = new System.Net.NetworkCredential("", ""); //todo skal have indsat Login oplysninger for værkstedet for den nye mail oprettet til projektet.
					//Set the timeout period for which the email keeps trying to send the email
					smtpServer.Timeout = 30000;
					//Put the email address
					mail.From = new MailAddress("@iha.dk"); //todo skal have indsat skolens statiske mail her.
					//Put the email where you want to send.
					mail.To.Add(component.ActualLoanInformation.OwnerID.ToString(CultureInfo.InvariantCulture) + "@iha.dk");
					mail.Subject = component.ComponentName + "indkaldes";

					StringBuilder sbBody = new StringBuilder();
					sbBody.AppendLine("Nu er tiden kommet til at få afleveret lånte komponenter fra værkstedet");
					sbBody.AppendLine("Du bedes returnere" + component.ComponentName + "snarest muligt");
					sbBody.AppendLine("Mvh Værkstedet i Shannon");

					//Writes the newly made predifned string builder into the mails body property.
					mail.Body = sbBody.ToString();
					//Your log file path
					//System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(@"C:\Logs\CheckoutPOS.log");
					//mail.Attachments.Add(attachment);

					smtpServer.Send(mail);
					//MessageBox.Show("The exception has been sent! :)");
				    component.ActualLoanInformation.IsEmailSend = "yes";
				    ldbUtil_.UpdateLoanInformation(component.ComponentID,component.ActualLoanInformation);
				}
				catch (Exception ex)
				{
					MessageBox.Show("Det var ikke muligt at sende emails med udløbet udlån" + ex);
					//Console.WriteLine(ex.ToString());
				}
			}
		}

		public void WaitForLoanInformationUpdated()
		{
			if (loanInformationUpdaterThread_.IsAlive)
			{
				var messageBoxText = "Vent venligst mens programmet indlæser komponenternes låne-information.";
				var caption = "Indlæser låne-informationer";
				var button = MessageBoxButton.OK;
				var icon = MessageBoxImage.Warning;

				do
				{
					MessageBox.Show(messageBoxText, caption, button, icon);
				} while (loanInformationUpdaterThread_.IsAlive);
				loanInformationUpdaterThread_.Join();
			}
		}

		#endregion

		#region Views/Windows

		//Loan component
		public void LoansWindow()
		{
			if (SelectedItem != null)
			{
				WaitForLoanInformationUpdated();

				if (SelectedItemList.Count == 1)
				{
					DateTime? startdate_ = new DateTime();
					DateTime? enddate_ = new DateTime();
					string studyNumber_ = SelectedItemList[0].ActualLoanInformation.ReservationID;
					string phoneNr_ = "";
					var locator = (ViewModelLocator)Application.Current.FindResource("Locator");
					locator.LoansViewModel = new LoanViewModel(startdate_, enddate_, studyNumber_, phoneNr_);

					Window dlg = new Loans();
					dlg.WindowStartupLocation = WindowStartupLocation.CenterScreen;
					if (dlg.ShowDialog() == true)
					{
						SelectedItem.ActualLoanInformation.ReturnDate = locator.LoansViewModel.EndDate;
						SelectedItem.ActualLoanInformation.LoanDate = locator.LoansViewModel.StartDate;
						SelectedItem.ActualLoanInformation.OwnerID = locator.LoansViewModel.StudyNumber;
						SelectedItem.ActualLoanInformation.MobilNr = locator.LoansViewModel.PhoneNr;
						SelectedItem.ActualLoanInformation.ReservationID = "";
						SelectedItem.ActualLoanInformation.ReservationDate = new SqlDateTime();

						if (false == cdbUtil_.UpdateComponent(SelectedItem.ComponentID, SelectedItem) ||
						   false == ldbUtil_.UpdateLoanInformation(SelectedItem.ComponentID, SelectedItem.ActualLoanInformation))
						{
							Reconnect();

						}
						SelectedItem.SetProperties(SelectedItem);
					}
				}
				else //more than one selected
				{
					DateTime? startdate_ = new DateTime();
					DateTime? enddate_ = new DateTime();
					string phoneNr_ = "";
					string studyNumber_ = SelectedItemList[0].ActualLoanInformation.ReservationID;
					var locator = (ViewModelLocator)Application.Current.FindResource("Locator");
					locator.LoansViewModel = new LoanViewModel(startdate_, enddate_, studyNumber_, phoneNr_);

					Window dlg = new Loans();
					dlg.WindowStartupLocation = WindowStartupLocation.CenterScreen;
					bool tryedReconnect = false; // Makes sure the reconnect message only appears once

					if (dlg.ShowDialog() == true)
					{
						for (int i = 0; i < SelectedItemList.Count; i++)
						{
							SelectedItemList[i].ActualLoanInformation.ReturnDate = locator.LoansViewModel.EndDate;
							SelectedItemList[i].ActualLoanInformation.LoanDate = locator.LoansViewModel.StartDate;
							SelectedItemList[i].ActualLoanInformation.OwnerID = locator.LoansViewModel.StudyNumber;
							SelectedItemList[i].ActualLoanInformation.MobilNr = locator.LoansViewModel.PhoneNr;
							SelectedItemList[i].ActualLoanInformation.ReservationID = "";
							SelectedItemList[i].ActualLoanInformation.ReservationDate = new SqlDateTime();

							if (false == cdbUtil_.UpdateComponent(SelectedItemList[i].ComponentID, SelectedItemList[i]) ||
							   false == ldbUtil_.UpdateLoanInformation(SelectedItemList[i].ComponentID, SelectedItemList[i].ActualLoanInformation))
							{
								if (!tryedReconnect)
								{
									Reconnect();
									tryedReconnect = true;
								}
							}
							SelectedItem.SetProperties(SelectedItem);
						}
					}
				}
			}
			else
			{
				var messageBoxText = "Du skal vælge en eller flere komponent(er) på listen.";
				var caption = "Ingen komponent valgt";
				var button = MessageBoxButton.OK;
				var icon = MessageBoxImage.Warning;

				var result = MessageBox.Show(messageBoxText, caption, button, icon);
			}

		}

		//Deliver a component
		public void DeliverComponent()
		{
	        if (SelectedItem != null)
		    {
		        WaitForLoanInformationUpdated();

	            if (SelectedItemList.Count == 1)
	            {
	                if (!string.IsNullOrEmpty(SelectedItem.ActualLoanInformation.OwnerID))
	                {
	                    var messageBoxText = "Er du sikker på du vil aflevere den valgte komponent med komponent navn: " +
	                                         SelectedItem.ComponentName + " og nummer: " + SelectedItem.ComponentNumber;
	                    var caption = "Aflever komponent";
	                    var button = MessageBoxButton.YesNo;
	                    var icon = MessageBoxImage.Question;

	                    var result = MessageBox.Show(messageBoxText, caption, button, icon);

	                    if (result == MessageBoxResult.Yes)
	                    {
	                        //Zero loaninformation locally and DB keep component and ID
	                        //DB:
	                        var loanInformation = new LoanInformation();
	                        loanInformation.LoanDate = SqlDateTime.Null;
	                        loanInformation.ReturnDate = SqlDateTime.Null;
	                        loanInformation.ReservationDate = SqlDateTime.Null;
	                        loanInformation.ReservationID = "";
	                        loanInformation.OwnerID = "";
	                        loanInformation.MobilNr = "";
	                        loanInformation.IsEmailSend = "no";
	                        ldbUtil_.UpdateLoanInformation(SelectedItem.ComponentID, loanInformation);

	                        //Local:
	                        SelectedItem.ActualLoanInformation.OwnerID = "";
	                        SelectedItem.ActualLoanInformation.LoanDate = null;
	                        SelectedItem.ActualLoanInformation.ReturnDate = null;
	                        SelectedItem.ActualLoanInformation.MobilNr = "";
	                        SelectedItem.ActualLoanInformation.ReservationDate = null;
	                        SelectedItem.ActualLoanInformation.ReservationID = "";
	                        SelectedItem.ActualLoanInformation.IsEmailSend = "no";

	                    }
	                }
	                else
	                {
	                    var messageBoxText = "Den valgte komponent er ikke udlånt.";
	                    var caption = "Aflever komponent";
	                    var button = MessageBoxButton.OK;
	                    var icon = MessageBoxImage.Warning;
	                    var result = MessageBox.Show(messageBoxText, caption, button, icon);
	                }
	            }
                else
	            {
	                var messageBoxText = "Er du sikker på du vil aflevere de valgte komponenter ?";
	                var caption = "Aflever komponenter";
	                var button = MessageBoxButton.YesNo;
	                var icon = MessageBoxImage.Question;

                    var result = MessageBox.Show(messageBoxText, caption, button, icon);

	                if (result == MessageBoxResult.Yes)
	                {
	                    //Zero loaninformation locally and DB keep component and ID
	                    //DB:
                        var loanInformation = new LoanInformation();
                        loanInformation.LoanDate = SqlDateTime.Null;
                        loanInformation.ReturnDate = SqlDateTime.Null;
                        loanInformation.ReservationDate = SqlDateTime.Null;
                        loanInformation.ReservationID = "";
                        loanInformation.OwnerID = "";
                        loanInformation.MobilNr = "";
                        loanInformation.IsEmailSend = "no";

	                    for (int i = 0; i < SelectedItemList.Count; i++)
	                    {
	                        if (!string.IsNullOrEmpty(SelectedItemList[i].ActualLoanInformation.OwnerID))
	                        {
	                            //DB:
	                            ldbUtil_.UpdateLoanInformation(SelectedItemList[i].ComponentID, loanInformation);
                                //Local:
	                            SelectedItemList[i].ActualLoanInformation.OwnerID = "";
	                            SelectedItemList[i].ActualLoanInformation.LoanDate = null;
	                            SelectedItemList[i].ActualLoanInformation.ReturnDate = null;
	                            SelectedItemList[i].ActualLoanInformation.MobilNr = "";
                                SelectedItemList[i].ActualLoanInformation.ReservationDate = null;
	                            SelectedItemList[i].ActualLoanInformation.ReservationID = "";
	                            SelectedItemList[i].ActualLoanInformation.IsEmailSend = "no";
	                        }
	                    }
	                }           
	            }
	        }
	    }


	    //Rename a category
		public void RenameCategory()
		{
			WaitForLoanInformationUpdated();

			var tempSelectedCat = SelectedCategory; //loses focus with inputbox, so have to write it down

			if (tempSelectedCat.ToLower() != "alle")
			{
				var ans =
					Microsoft.VisualBasic.Interaction.InputBox(
						"Du er i færd med at omdøbe den valgte kategori: " + SelectedCategory +
						". Hvis du vil fortsætte, skriv navnet på en ny kategori, som skal erstatte den gamle.",
						"Omdøb kategori", ""); //referencing Microsoft.VisualBasic

				if (ans.Length != 0 && !Categories.Contains(ans) && !ans.ToLower().Contains("alle"))
				{
					//remove the old, add the new category
					var cat = (Categories)Application.Current.FindResource("Categories");
					cat.Remove(tempSelectedCat);
					cat.Add(ans);

					//update all components with the old category to the new category
					var update = GetComponentsFromCategory(tempSelectedCat);
					for (int i = 0; i < update.Count; i++)
					{
						update[i].Category = ans;

						if (false == cdbUtil_.UpdateComponent(update[i].ComponentID, update[i]))
						{
							Reconnect();
						}
					}
					Refresh();
				}
				else
					MessageBox.Show(
						"Du skal skrive navnet på en ny kategori. Du må ikke omdøbe til en kategori der allerede eksisterer.");
			}
			else
			{
				MessageBox.Show("Du må ikke omdøbe kategorien 'Alle'");
			}
		}

		//Delete selected components
		//This is not a part of the viewmodel http://stackoverflow.com/questions/10227007/are-you-sure-prompts-part-of-the-viewmodel-or-purely-the-view
		public void DeleteSelectedComponent()
		{
			WaitForLoanInformationUpdated();

			if (SelectedItemList.Count == 1)
			{
				// Configure the message box to be displayed 
				var messageBoxText = "Er du sikker på du vil slette den valgte komponent med komponent navn: " +
									 SelectedItem.ComponentName + " og nummer: " + SelectedItem.ComponentNumber;
				var caption = "Slet komponent";
				var button = MessageBoxButton.YesNo;
				var icon = MessageBoxImage.Question;

				var result = MessageBox.Show(messageBoxText, caption, button, icon);

				if (result == MessageBoxResult.Yes)
				{
					if (false == cdbUtil_.DeleteComponent(selectedItem_.ComponentID))
					{
						Reconnect();
					}
					AllComponentList.Remove(SelectedItem);
				}
			}
			else if (SelectedItemList.Count > 1)
			{
				var messageBoxText = "Du har valgt mere end en komponent, er du sikker på du vil slette disse?";
				var caption = "Slet flere komponenter";
				var button = MessageBoxButton.YesNo;
				var icon = MessageBoxImage.Question;

				var result = MessageBox.Show(messageBoxText, caption, button, icon);

				if (result == MessageBoxResult.Yes)
				{
					for (int i = 0; i < SelectedItemList.Count; i++)
					{
						if (false == cdbUtil_.DeleteComponent(selectedItemList_[i].ComponentID))
						{
							Reconnect();
						}
						AllComponentList.Remove(SelectedItemList[i]);
					}
				}
			}
			else
			{
				var messageBoxText = "Du har ikke valgt en komponent på listen. Ingen komponenter er slettet.";
				var caption = "Ingen komponent valgt";
				var button = MessageBoxButton.OK;
				var icon = MessageBoxImage.Warning;
				MessageBox.Show(messageBoxText, caption, button, icon);
			}
			SetComponentList(AllComponentList); //Set the content of the componentdatagrid
			CategoryIndex = 0;
		}

		//Create component(s)
		public void CreateComponentWindow()
		{
			WaitForLoanInformationUpdated();

			if (SelectedItem != null)
			{
				//Copy the selected into the new view, for easy adding of more of the same
				newComponent_ = new Component(SelectedItem);
				newComponent_.ComponentNumber = GetNextComponentNumber(SelectedItem.ComponentName);
			}
			else
				newComponent_ = new Component { ComponentNumber = 1 };

			//Remove "Null" text when creating new component
			newComponent_.ActualLoanInformation.LoanDate = null;
			newComponent_.ActualLoanInformation.ReturnDate = null;

			//Locator locate our CreateComponentViewModel, then set the CreateComponentViewModel equal to a new one with newComponent_ inside:
			var locator = (ViewModelLocator)Application.Current.FindResource("Locator");
			locator.CreateComponentViewModel = new CreateComponentViewModel(newComponent_);
			locator.CreateComponentViewModel.AddThisManyComponents = 1;

			Window dlg = new CreateComponent();
			dlg.DataContext = locator.CreateComponentViewModel;
			dlg.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			bool tryedReconnect = false; // Makes sure the reconnect message only appears once
			//If user press ok then add all the components
			if (dlg.ShowDialog() == true)
			{
			    if (!string.IsNullOrEmpty(locator.CreateComponentViewModel.NewComponent.ComponentName))
			    {

			        var startNumber = locator.CreateComponentViewModel.NewComponent.ComponentNumber;
			        for (int i = 0; i < locator.CreateComponentViewModel.AddThisManyComponents; i++)
			        {
			            var tempComponent = new Component(newComponent_); //copy the newComponent
			            tempComponent.ComponentNumber = startNumber + i; //change the component number accordingly

			            if (false == cdbUtil_.CreateComponent(tempComponent))
			            {
			                if (!tryedReconnect)
			                {
			                    Reconnect();
			                    tryedReconnect = true;
			                }
			            }
			            AllComponentList.Add(tempComponent);
			        }
			    }
                else
                {
                    MessageBox.Show("Du skal indtaste et komponent-navn.");
                }
			}
			
			SetComponentList(AllComponentList); //Set the content of the componentdatagrid
			CategoryIndex = 0;
			//then or else dischard the data the user inputted:
			newComponent_ = null; //redundant? Don't ref the old one, rather some new uninitted
		}

		//Log in window
		public void LogInWindow()
		{
			var locator = new ViewModelLocator();

			Window dlg = new LogIn();
			dlg.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			dlg.DataContext = locator.NewLogInViewModel;
			if (dlg.ShowDialog() == true)
			{
				LoggedIn = true;
				LoggedOut = false;

			}
		}

		//Change email
		public void ChangeEmail()
		{
			Window dlg = new ChangeEmail();
			dlg.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			if (dlg.ShowDialog() == true)
			{
				Properties.Settings.Default.Save();
			}
		}

		//Change componenet(s)
		public void ChangeComponentWindow()
		{
			if (SelectedItem != null)
			{
				WaitForLoanInformationUpdated();
				if (SelectedItemList.Count == 1)
				{
					var tempComponent = new Component(SelectedItem); //copy of selected
					var locator = (ViewModelLocator)Application.Current.FindResource("Locator");
					locator.NewChangeComponentViewModel = new ChangeComponentViewModel(tempComponent);

					Window dlg = new ChangeComponent();
					dlg.WindowStartupLocation = WindowStartupLocation.CenterScreen;
					dlg.DataContext = locator.NewChangeComponentViewModel;

					if (dlg.ShowDialog() == true)
					{
						//If user changed reservation date
						if (locator.NewChangeComponentViewModel.RemoveReservationCheck)
						{
							tempComponent.ActualLoanInformation.ReservationID = "";
							tempComponent.ActualLoanInformation.ReservationDate = new SqlDateTime();
						}

						if (false == cdbUtil_.UpdateComponent(SelectedItem.ComponentID, tempComponent) ||
							false == ldbUtil_.UpdateLoanInformation(SelectedItem.ComponentID, tempComponent.ActualLoanInformation))
						{
							Reconnect();
						}


						SelectedItem.SetProperties(tempComponent);
					}
				}
				else //more than one selected
				{
					//copy of the first but don't show component number (zero)
					var tempComponent = new Component(SelectedItemList[0]);
					tempComponent.ComponentNumber = 0;

					//Create the viewmodel give it the tempComponent
					var locator = (ViewModelLocator)Application.Current.FindResource("Locator");
					locator.NewChangeComponentViewModel = new ChangeComponentViewModel(tempComponent);

					//Disable editing the component number
					locator.NewChangeComponentViewModel.EditComponentNumber = false;

					//Create the view
					Window dlg = new ChangeComponent();
					dlg.WindowStartupLocation = WindowStartupLocation.CenterScreen;
					dlg.DataContext = locator.NewChangeComponentViewModel;
					bool triedReconnect = false; // Makes sure the reconnect message only appears once
					if (dlg.ShowDialog() == true)
					{
						LockApplication();	//reverse by logging in

						var t = new Thread(() =>
						{
							//Set up load bar:
							MaxLoadbar = SelectedItemList.Count;
							StatusBarText = "Opdaterer komponenter...";
							LoadBarVisibility = Visibility.Visible;

							for (int i = 0; i < SelectedItemList.Count; i++)
							{
								//Set component ID and Component Number equal to the one being edited because this should not be edited
								tempComponent.ComponentID = SelectedItemList[i].ComponentID;
								tempComponent.ComponentNumber = SelectedItemList[i].ComponentNumber;

								if (false == cdbUtil_.UpdateComponent(SelectedItemList[i].ComponentID, tempComponent) ||
									false == ldbUtil_.UpdateLoanInformation(SelectedItemList[i].ComponentID, tempComponent.ActualLoanInformation))
								{
									if (!triedReconnect)
									{
										Reconnect();
										triedReconnect = true;
									}
								}
								SelectedItemList[i].SetProperties(tempComponent);
								LoadbarProgress++;
							}

							//Clean load bar:
							StatusBarText = "Klar";
							LoadbarProgress = 0;
							LoadBarVisibility = Visibility.Hidden;
							LogIn();
						});
						t.Start();
					}
				}
			}
			else
			{
				var messageBoxText = "Du skal vælge en eller flere komponent(er) på listen.";
				var caption = "Ingen komponent valgt";
				var button = MessageBoxButton.OK;
				var icon = MessageBoxImage.Warning;

				var result = MessageBox.Show(messageBoxText, caption, button, icon);
			}
		}

		#endregion
	}
}