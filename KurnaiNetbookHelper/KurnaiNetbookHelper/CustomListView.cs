using System.Windows.Forms;

// Source: Adnan Dedic - http://social.msdn.microsoft.com/Forums/vstudio/en-US/a20954f5-190d-4baf-8f74-d4435ca4623a/c-lisview-scroll-click-event?forum=csharpgeneral
public class CustomListView : ListView
{
    // The following code adds an "OnScroll" event handler.
    private const int WM_HSCROLL = 0x114;
    private const int WM_VSCROLL = 0x115;
    public event ScrollEventHandler OnScroll;

    protected virtual void OnScrollChanged(ScrollEventArgs e)
    {
        ScrollEventHandler handler = this.OnScroll;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);
        if (m.Msg == WM_VSCROLL || m.Msg == WM_HSCROLL)
        {
            OnScrollChanged(new ScrollEventArgs((ScrollEventType)(m.WParam.ToInt32() & 0xffff), 0));
        }
    }

    // The following code prevents items from being checked via double-clicking.
    // Source: http://stackoverflow.com/a/6304718
    private bool checkFromDoubleClick = false;

    protected override void OnItemCheck(ItemCheckEventArgs ice)
    {
        if (this.checkFromDoubleClick)
        {
            ice.NewValue = ice.CurrentValue;
            this.checkFromDoubleClick = false;
        }
        else
            base.OnItemCheck(ice);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        // Is this a double-click?
        if ((e.Button == MouseButtons.Left) && (e.Clicks > 1))
        {
            this.checkFromDoubleClick = true;
        }
        base.OnMouseDown(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        this.checkFromDoubleClick = false;
        base.OnKeyDown(e);
    }

}
