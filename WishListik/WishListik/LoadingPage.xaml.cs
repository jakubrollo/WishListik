using System;
using System.Timers;
using Microsoft.Maui.Controls;

namespace WishListik
{
    // Represents a page displaying a loading animation
    public partial class LoadingPage : ContentPage
    {
        private System.Timers.Timer _timer; // Timer for controlling the animation interval
        private int _dotCount = 0; // Counter for animation dots

        // Ctor for LoadingPage
        public LoadingPage()
        {
            InitializeComponent();
            _timer = new System.Timers.Timer(500); // Set up the timer with a 500ms interval
            StartLoadingAnimation(); // Begin the loading animation
        }

        // Starts the loading animation by setting up the timer
        private void StartLoadingAnimation()
        {
            _timer.Elapsed += OnTimerElapsed; // Subscribe to the timer's elapsed event
            _timer.Start();
        }

        // Updates the loading animation periodically
        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                _dotCount = (_dotCount + 1) % 4; // Rotate the dot count
                LoadingLabel.Text = $"Loading{new string('.', _dotCount)}"; // Update the label text
            });
        }
    }
}
