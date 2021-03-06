﻿using SmartClock.InfoProviders.XinZhiWeatherInfoProvider;
using SmartClock.JSClockManager;
using SmartClock.NetcoreRenderer;
using SmartClock.UWPRenderer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPTest1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const string CLOCK_NAME = "TestClock";
        Manager manager = new Manager();
        //SmartClock.UWPRenderer.ImageSourceRenderer render = new SmartClock.UWPRenderer.ImageSourceRenderer();
        //SmartClock.UWPRenderer.WaveShareEink32Renderer einkRender = new SmartClock.UWPRenderer.WaveShareEink32Renderer();
        CombinedRenderer render = new CombinedRenderer();
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            refreshPacks();
            ImageSourceRenderer imgRenderer = new ImageSourceRenderer();
            this.imgResult.DataContext = imgRenderer.Data;
            render.Renderers.Add(new CombinedRendererItem(imgRenderer));
            WaveShareEink32Renderer einkRenderer = new WaveShareEink32Renderer();
            render.Renderers.Add(new CombinedRendererItem(einkRenderer, false));
            RemoteRenderer remoteRenderer = new RemoteRenderer("192.168.0.220");
            render.Renderers.Add(new CombinedRendererItem(remoteRenderer, false));
            lstRenders.ItemsSource = render.Renderers;
            var xinzhi = new XinzhiWeatherForcast("gxs3ezcux67dzvqa", "shanghai");//replace the key with your own, this is for my development only
            xinzhi.Start();
            manager.InfoManager.Providers.Add(xinzhi);
            manager.AddDefinition(CLOCK_NAME, render, SmartClock.Core.ClockRefreshIntervalEnum.PerSecond);
        }

        private void refreshPacks()
        {
            var items= manager.InstalledClockScripts;
            this.lstPacks.ItemsSource = items;
            if (items.Count()>0)
            {
                lstPacks.SelectedIndex = 0;
            }
            
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            ClockScriptInfo info = lstPacks.SelectedItem as ClockScriptInfo;
            btnLoad.IsEnabled = false;
            manager.StartClock(CLOCK_NAME, info.FolderName);
            btnLoad.IsEnabled = true;

        }
    }
}
