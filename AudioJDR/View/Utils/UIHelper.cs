using View.Resources.Localization;

/// <summary>
/// Provides utility methods for UI operations across pages
/// </summary>
public static class UIHelper
{
    #region Constants
    
    /// <summary>
    /// Struct containting all size constants
    /// </summary>
    public readonly struct Sizes
    {
        // Common sizes
        public const double MIN_BUTTON_WIDTH = 200;
        public const double MIN_BUTTON_HEIGHT = 40;
        public const double MIN_FRAME_WIDTH = 300;
        public const double MIN_EDITOR_HEIGHT = 200;
        
        // Relative sizing factors
        public const double BUTTON_WIDTH_FACTOR = 0.25;
        public const double BUTTON_HEIGHT_FACTOR = 0.08;
        public const double FRAME_WIDTH_FACTOR = 0.9;
        public const double LIST_HEIGHT_FACTOR = 0.5;
        public const double BACK_BUTTON_WIDTH_FACTOR = 0.8;
        public const double FONT_SIZE_FACTOR = 0.1;
        public const double MAX_FONT_SIZE = 18;
    }

    #endregion

    #region Dialog Methods

    /// <summary>
    /// Shows a confirmation dialog for unsaved changes
    /// </summary>
    public static async Task<bool> ShowUnsavedChangesDialog(Page page)
    {
        return await page.DisplayAlert(
            AppResources.Confirm,
            AppResources.UnsavedChangesMessage,
            AppResources.Yes,
            AppResources.No
        );
    }

    /// <summary>
    /// Shows a confirmation dialog for deletion of any item
    /// </summary>
    /// <param name="page">The current page</param>
    /// <param name="itemName">Name of the item to be deleted</param>
    public static async Task<bool> ShowDeleteConfirmationDialog(Page page, string itemName)
    {
        string message = string.Format(AppResources.DeleteConfirmationFormat, itemName);
        return await page.DisplayAlert(
            AppResources.Confirm,
            message,
            AppResources.Yes,
            AppResources.No
        );
    }

    /// <summary>
    /// Shows a success message dialog with custom message
    /// </summary>
    public static async Task ShowSuccessDialog(Page page, string message)
    {
        await page.DisplayAlert(
            AppResources.Success,
            message,
            AppResources.OK
        );
    }

    /// <summary>
    /// Shows an error message dialog with custom message
    /// </summary>
    public static async Task ShowErrorDialog(Page page, string message)
    {
        await page.DisplayAlert(
            AppResources.Error,
            message,
            AppResources.OK
        );
    }

    #endregion

    #region UI Sizing Methods

    /// <summary>
    /// Calculates and returns the frame width based on page width
    /// </summary>
    public static double GetResponsiveFrameWidth(double pageWidth)
    {
        return Math.Max(pageWidth * Sizes.FRAME_WIDTH_FACTOR, Sizes.MIN_FRAME_WIDTH);
    }

    /// <summary>
    /// Sets the size of a button based on its type
    /// </summary>
    public static void SetButtonSize(Page page, Button button, bool isBackButton)
    {
        double pageWidth = page.Width; 
        double pageHeight = page.Height; 

        double buttonWidth = Math.Max(pageWidth * Sizes.BUTTON_WIDTH_FACTOR, Sizes.MIN_BUTTON_WIDTH);
        double buttonHeight = Math.Max(pageHeight * Sizes.BUTTON_HEIGHT_FACTOR, Sizes.MIN_BUTTON_HEIGHT);
        double buttonFontSize = Math.Min(buttonWidth * Sizes.FONT_SIZE_FACTOR, Sizes.MAX_FONT_SIZE);

        button.WidthRequest = buttonWidth;
        button.HeightRequest = buttonHeight;
        button.FontSize = buttonFontSize;

        if (isBackButton)
        {
            button.WidthRequest *= Sizes.BACK_BUTTON_WIDTH_FACTOR; // Adjust width for back button
        }
    }

    #endregion
} 