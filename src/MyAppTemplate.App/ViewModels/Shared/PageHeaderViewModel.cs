namespace MyAppTemplate.App.ViewModels.Shared;

public class PageHeaderViewModel
{
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? Icon { get; set; }
    public List<BreadcrumbItem> BreadcrumbItems { get; set; } = new();
    public List<PageHeaderActionButton> ActionButtons { get; set; } = new();
    public string? ActionButtonText { get; set; }
    public string? ActionButtonUrl { get; set; }
    public string? ActionButtonIcon { get; set; }

    public PageHeaderViewModel WithTitle(string title, string? subtitle = null, string? icon = null)
    {
        Title = title;
        Subtitle = subtitle;
        Icon = icon;
        return this;
    }

    public PageHeaderViewModel AddBreadcrumb(string title, string? url = null, string? icon = null)
    {
        BreadcrumbItems.Add(new BreadcrumbItem
        {
            Title = title,
            Url = url,
            Icon = icon
        });
        return this;
    }

    public PageHeaderViewModel AddActionButton(
        string text,
        string? cssClass = null,
        string? icon = null,
        string? url = null,
        IDictionary<string, string>? attributes = null,
        string? buttonType = null,
        string? id = null)
    {
        ActionButtons.Add(new PageHeaderActionButton
        {
            Text = text,
            CssClass = string.IsNullOrWhiteSpace(cssClass) ? "btn-primary-custom" : cssClass,
            Icon = icon,
            Url = url,
            Attributes = attributes,
            ButtonType = string.IsNullOrWhiteSpace(buttonType) ? "button" : buttonType,
        });
        return this;
    }

    public class BreadcrumbItem
    {
        public string Title { get; set; } = string.Empty;
        public string? Url { get; set; }
        public string? Icon { get; set; }
        public bool IsActive => string.IsNullOrEmpty(Url);
    }

    public class PageHeaderActionButton
    {
        public string Text { get; set; } = string.Empty;
        public string CssClass { get; set; } = "btn-primary-custom";
        public string? Icon { get; set; }
        public string? Url { get; set; }
        public IDictionary<string, string>? Attributes { get; set; }
        public string ButtonType { get; set; } = "button";
    }
}
