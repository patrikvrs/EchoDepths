using Godot;

public partial class TutorialTextChange : Area3D
{
    private Control _tutorialUI;
    private Label _tutorialLabel;

    [Export(PropertyHint.MultilineText)]
    private string _newTutorialText = "This is the new tutorial text.";

    public override void _Ready()
    {
        _tutorialUI = GetNode<Control>("/root/TutorialMap/tutorial_UI");
        _tutorialLabel = _tutorialUI.GetNode<Label>("Tutorial_Label");
        BodyEntered += OnBodyEntered;
    }

    public void OnBodyEntered(Node3D body)
    {
        if (body is Player && _tutorialLabel != null)
        {
            _tutorialLabel.Text = _newTutorialText;
        }
    }
}