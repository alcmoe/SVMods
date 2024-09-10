using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace VideoFrame.UI;

public class AlbumMenu : PaginatedMenu
{
    private UiElement? _playButton;
    private UiElement? _tra1Button;
    private UiElement? _tra2Button;
    private int _lastIndex;
    private int _playMod;
    private bool _playing;
    private readonly float _duration;
    private readonly Rectangle _playButtonSourceRect = new(175, 379, 16, 15);
    private readonly Rectangle _triangle1SourceRect = new(448, 96, 32, 32);
    private readonly Rectangle _triangle2SourceRect = new(480, 96, 32, 32);
    

    public AlbumMenu(string name, List<MenuPage> pages, Rectangle bounds, float duration, int topEdgeSize = 4, int bottomEdgeSize = 4, int leftEdgeSize = 4, int rightEdgeSize = 4) :
        base(name, pages, bounds, DrawableType.None, "bigSelect", null, null, null, topEdgeSize, bottomEdgeSize, leftEdgeSize, rightEdgeSize)
    {
        _duration = duration;
    }

    protected override void OrganiseUi(Orientation orientation)
    {
        base.OrganiseUi(orientation);
        var playButtonX = (int)BottomLeftCorner.X + Width / 2 + 20;
        var playButtonBound = new Rectangle(playButtonX, (int)BottomLeftCorner.Y, _triangle1SourceRect.Width, _triangle1SourceRect.Height);
        var tra1X = (int)BottomLeftCorner.X + Width / 2 - 20 - _triangle1SourceRect.Width * 2;
        var tra1Bound = new Rectangle(tra1X, (int)BottomLeftCorner.Y, _triangle1SourceRect.Width, _triangle1SourceRect.Height);
        var tra2X = (int)BottomLeftCorner.X + Width / 2 - 20 - _triangle2SourceRect.Width * 1;
        var tra2Bound = new Rectangle(tra2X, (int)BottomLeftCorner.Y, _triangle2SourceRect.Width, _triangle2SourceRect.Height);
        if (_playButton == null || _tra1Button == null || _tra2Button == null) {
            _playButton = new UiElement("Play Button", playButtonBound, DrawableType.Texture, Game1.mouseCursors, _playButtonSourceRect, scale: 1,drawShadow: true);
            _tra1Button = new UiElement("Play Mod Button1", tra1Bound, DrawableType.Texture, Game1.mouseCursors, _triangle1SourceRect, scale: 1);
            _tra2Button = new UiElement("Play Mod Button2", tra2Bound, DrawableType.Texture, Game1.mouseCursors, _playMod == 0 ? _triangle1SourceRect : _triangle2SourceRect, scale: 1);
            _playButton.LeftClickCallback = PlayButtonClicked;
            _tra1Button.LeftClickCallback = PlayModClicked;
            _tra2Button.LeftClickCallback = PlayModClicked;
        } else {
            _playButton.Bounds = playButtonBound;
            _tra1Button.Bounds = tra1Bound;
            _tra2Button.Bounds = tra2Bound;
        }
    }

    private void Play()
    {
        _playing = true;
        SetDrawUi(false);
        _lastIndex = Index;
    }

    private void Pause()
    {
        _playing = false;
        SetDrawUi(true);
        Index = _lastIndex;
    }

    private void PlayButtonClicked()
    {
        Play();
    }

    private void PlayModClicked()
    {
        if (_playMod == 0) {
            _playMod = 1;
            _tra2Button!.SourceRect = _triangle2SourceRect;
        } else {
            _playMod = 0;
            _tra2Button!.SourceRect = _triangle1SourceRect;
        }
    }

    public override void ReceiveLeftClick(int x, int y)
    {
        if (_playing) {
            Pause();
            return;
        }
        if (_playButton!.Bounds.Contains(x, y))
            _playButton.ReceiveLeftClick(x, y);
        if (_tra1Button!.Bounds.Contains(x, y))
            _tra1Button!.ReceiveLeftClick(x, y);
        if (_tra2Button!.Bounds.Contains(x, y))
            _tra2Button!.ReceiveLeftClick(x, y);
        base.ReceiveLeftClick(x, y);
    }

    public override void ReceiveScrollWheel(int direction)
    {        
        if (_playing) {
            Pause();
            return;
        }
        base.ReceiveScrollWheel(direction);
    }

    internal override void Draw(SpriteBatch spriteBatch)
    {
        if (DrawUi) {
            _playButton!.Draw(spriteBatch, Color.White);
            _tra1Button!.Draw(spriteBatch, Color.White);
            _tra2Button!.Draw(spriteBatch, Color.White);
        }
        if (_playing) {
            if (Utils.TimeIsMultipleOf((uint)(_duration * 60))) {
                if (_playMod == 0) {
                    var last = Index;
                    Index += 1;
                    if (last == Index) {
                        Index = 0;
                    }
                } else if (_playMod == 1) {
                    Index = new Random().Next(0, Pages.Count);
                }
            }

        }
        base.Draw(spriteBatch);
    }
}
