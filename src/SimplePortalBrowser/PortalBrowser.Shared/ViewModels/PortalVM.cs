﻿using Esri.ArcGISRuntime.Portal;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PortalBrowser.ViewModels
{

    
    /// <summary>
    /// Portal View Model that handles all logic related to the portal object
    /// </summary>
	public class PortalVM : BaseViewModel
	{
        public PortalVM()
        {
            Initialize();
        }

        private async void Initialize()
        {
            try
            {
                await LoadPortal();
            }
            catch (System.Exception ex)
            {
                StatusMessage = ex.Message;
            }
        }
            /// <summary>
            /// Async task to load a portal instance
            /// </summary>
            private async Task LoadPortal()
		{
			StatusMessage = "Initializing Portal...";
			var portal = await ArcGISPortal.CreateAsync();
			PortalInfo = portal.ArcGISPortalInfo;
			IsLoadingPortal = false;
			await LoadMaps(portal);
		}

        /// <summary>
        /// Async task to load maps from the portal instance and put them in groups
        /// </summary>
        /// <param name="portal">Portal instance</param>
        /// <returns></returns>
		private async Task LoadMaps(ArcGISPortal portal)
		{
			StatusMessage = "Loading maps...";
			
			var task1 = portal.ArcGISPortalInfo.SearchBasemapGalleryAsync();
			var task2 = portal.ArcGISPortalInfo.SearchFeaturedItemsAsync(new SearchParameters("") { Limit = 50 });
			var items = await task1;
			Basemaps = items.Results;
			var groups = new ObservableCollection<MapGroup>();
			Groups = groups;
			groups.Add(new MapGroup() { Name = "Base maps", Items = Basemaps });
			IsLoadingBasemaps = false;
			StatusMessage = string.Format("Connected to {0} ({1})", portal.ArcGISPortalInfo.PortalName, portal.ArcGISPortalInfo.PortalHostname);
			items = await task2;
			Featured = items.Results;
			groups.Add(new MapGroup() { Name = "Featured", Items = Featured });
			Groups = groups;
		}

		private ArcGISPortalInfo m_portalInfo;
        /// <summary>
        /// Property holds information about the loaded portal 
        /// </summary>
		public ArcGISPortalInfo PortalInfo
		{
			get { return m_portalInfo; }
			set
			{
				m_portalInfo = value;
				OnPropertyChanged("PortalInfo");
			}
		}

		private IEnumerable<MapGroup> m_groups;
        /// <summary>
        /// Groups property to hold the map groups created from the portal
        /// </summary>
		public IEnumerable<MapGroup> Groups
		{
			get { return m_groups; }
			set
			{
				m_groups = value;
				OnPropertyChanged("Groups");
			}
		}
		
		private IEnumerable<ArcGISPortalItem> m_Basemaps;
        /// <summary>
        /// Property holding the list of basemaps to be added to the UI
        /// </summary>
		public IEnumerable<ArcGISPortalItem> Basemaps
		{
			get { return m_Basemaps; }
			set
			{
				m_Basemaps = value;
				OnPropertyChanged("Basemaps");
			}
		}

		private IEnumerable<ArcGISPortalItem> m_Featured;
        /// <summary>
        /// Property holding the list of featured maps to be added to the UI
        /// </summary>
		public IEnumerable<ArcGISPortalItem> Featured
		{
			get { return m_Featured; }
			set
			{
				m_Featured = value;
				OnPropertyChanged("Featured");
			}
		}

		private string m_StatusMessage = "OK";
        /// <summary>
        /// Property holding the status message to inform user of progress
        /// </summary>
		public string StatusMessage
		{
			get { return m_StatusMessage; }
			set
			{
				m_StatusMessage = value;
				OnPropertyChanged("StatusMessage");
				System.Diagnostics.Debug.WriteLine(value);
			}
		}

		private bool m_IsLoadingPortal = true;
        /// <summary>
        /// Boolean to reflect whether the portal has finished loading
        /// </summary>
		public bool IsLoadingPortal
		{
			get { return m_IsLoadingPortal; }
			set
			{
				m_IsLoadingPortal = value;
				OnPropertyChanged("IsLoadingPortal");
				OnPropertyChanged("IsLoading");
			}
		}

		private bool m_IsLoadingBasemaps = true;
        /// <summary>
        /// Boolean to reflect whether the basemaps have finished loading
        /// </summary>
		public bool IsLoadingBasemaps
		{
			get { return m_IsLoadingBasemaps; }
			set
			{
				m_IsLoadingBasemaps = value;
				OnPropertyChanged("IsLoadingBasemaps");
				OnPropertyChanged("IsLoading");
			}
		}

        /// <summary>
        /// Composite property to reflect if any item is currently loading
        /// </summary>
		public bool IsLoading
		{
			get
			{
				return IsLoadingBasemaps && IsLoadingPortal;
			}
		}
	}
    /// <summary>
    /// Class that holds map groups with name and map items
    /// </summary>
	public class MapGroup
	{
		public string Name { get; set; }
		public IEnumerable<ArcGISPortalItem> Items { get; set; }
	}
}