﻿using System.Windows;

namespace Assistant.Desktop;

public partial class MainWindow : Window
{
    public IServiceProvider Services { get; }

    public MainWindow(IServiceProvider services)
    {
        Services = services;
        InitializeComponent();
        DataContext = this;
    }
}