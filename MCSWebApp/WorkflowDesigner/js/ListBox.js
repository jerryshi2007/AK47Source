
function ListBox(Arguments) {
    this.Version = '1.0';
    var Ids = 0;
    var EventHandlers = new Array();

    var Base = Arguments.Base ? Arguments.Base : document.documentElement;
    var Height = Arguments.Height ? Arguments.Height : 132;
    var Width = Arguments.Width ? Arguments.Width : 300;

    var ClickEventHandler = Arguments.ClickEventHandler ? Arguments.ClickEventHandler : function () { };

    var ListBoxDiv = document.createElement('div');

    ListBoxDiv.className = "ListBoxDiv";

    ListBoxDiv.style.setAttribute("width", Width + "px");
    ListBoxDiv.style.setAttribute("height", Height + "px");

    this.AddItem = function (_text, _value, _checked) {
        var Item = null;
        var CheckBox = null;
        var Span = null;

        Item = document.createElement('div');
        if (Ids % 2 == 0)
            Item.className = "NormalItemListBoxDiv";
        else
            Item.className = "AlternateItemListBoxDiv";

        Item.ItemIndex = Ids;

        CheckBox = document.createElement("input");
        CheckBox.setAttribute("type", "checkbox");
        CheckBox.className = "checkBoxStyle";
        // CheckBox.setAttribute("class", "checkBoxStyle");

        Item.appendChild(CheckBox);
        if (_checked) {
            CheckBox.setAttribute("checked", "checked");
            CheckBox.checked = true;
        }

        Span = document.createElement('span');
        Span.innerText = _text;
        Span.value = _value;
        Span.title = _text;
        Item.appendChild(Span);

        ListBoxDiv.appendChild(Item);

        //Register events.
        //        WireUpEventHandler(Item, 'mouseover', function () { OnMouseOver(CheckBox, Item); });
        //        WireUpEventHandler(Item, 'mouseout', function () { OnMouseOut(CheckBox, Item); });
        WireUpEventHandler(Item, 'selectstart', function () { return false; });
        // WireUpEventHandler(CheckBox, 'click', function () { OnClick(CheckBox, Item); });
        WireUpEventHandler(CheckBox, 'click', function () { ClickEventHandler(CheckBox, { IsSelected: CheckBox.checked, Text: _text, Value: _value, ItemIndex: Item.ItemIndex }); });


        Ids++;
    }

    //Public method GetItems.
    this.GetItems = function () {
        var Items = new Array();

        var Divs = ListBoxDiv.getElementsByTagName('div');

        for (var n = 0; n < Divs.length; ++n)
            Items.push({ IsSelected: Divs[n].childNodes[0].checked, Text: Divs[n].childNodes[1].innerHTML, Value: Divs[n].childNodes[1].value, ItemIndex: Divs[n].ItemIndex });

        return Items;
    }

    //Public method Dispose.
    this.Dispose = function () {
        while (EventHandlers.length > 0)
            DetachEventHandler(EventHandlers.pop());

        Base.removeChild(ListBoxDiv);
    }

    //Public method Contains.
    this.Contains = function (Index) {
        return typeof (Index) == 'number' && ListBoxDiv.childNodes[Index] ? true : false;
    }

    //Public method GetItem.
    this.GetItem = function (Index) {
        var Divs = ListBoxDiv.getElementsByTagName('div');

        return this.Contains(Index) ? { IsSelected: Divs[Index].childNodes[0].checked, Text: Divs[Index].childNodes[1].innerHTML, Value: Divs[Index].childNodes[1].value, ItemIndex: Index} : null;
    }

    //Public method DeleteItem.
    this.DeleteItem = function (Index) {
        if (!this.Contains(Index)) return false;

        try {
            ListBoxDiv.removeChild(ListBoxDiv.childNodes[Index]);
        }
        catch (err) {
            return false;
        }

        return true;
    }

    //Public method DeleteItems.
    this.DeleteItems = function () {
        var ItemsRemoved = 0;

        for (var n = ListBoxDiv.childNodes.length - 1; n >= 0; --n)
            try {
                ListBoxDiv.removeChild(ListBoxDiv.childNodes[n]);
                ItemsRemoved++;
            }
            catch (err) {
                break;
            }

        return ItemsRemoved;
    }

    //Public method GetTotalItems.
    this.GetTotalItems = function () {
        return ListBoxDiv.childNodes.length;
    }

    //Item mouseover event handler.
    var OnMouseOver = function (CheckBox, Item) {
        if (CheckBox.checked) return;
    }

    //Item mouseout event handler.
    var OnMouseOut = function (CheckBox, Item) {
        if (CheckBox.checked) return;
    }

    //CheckBox click event handler.
    var OnClick = function (CheckBox, Item) {
        if (CheckBox.checked) {
            Item.style.backgroundColor = SelectedIItemBackColor;
            Item.style.color = SelectedItemColor;
            Item.style.borderTopColor = Item.style.borderBottomColor = SelectedIItemBackColor;
        }
        else {
            Item.style.color = null;
            Item.style.backgroundColor = HoverItemBackColor;
            Item.style.color = HoverItemColor;
            Item.style.borderTopColor = Item.style.borderBottomColor = HoverBorderdColor;
        }
    }

    //Private anonymous method to wire up event handlers.
    var WireUpEventHandler = function (Target, Event, Listener) {
        //Register event.
        if (Target.addEventListener)
            Target.addEventListener(Event, Listener, false);
        else if (Target.attachEvent)
            Target.attachEvent('on' + Event, Listener);
        else {
            Event = 'on' + Event;
            Target.Event = Listener;
        }

        //Collect event information through object literal.
        var EVENT = { Target: Target, Event: Event, Listener: Listener }
        EventHandlers.push(EVENT);
    }

    //Private anonymous  method to detach event handlers.
    var DetachEventHandler = function (EVENT) {
        if (EVENT.Target.removeEventListener)
            EVENT.Target.removeEventListener(EVENT.Event, EVENT.Listener, false);
        else if (EVENT.Target.detachEvent)
            EVENT.Target.detachEvent('on' + EVENT.Event, EVENT.Listener);
        else {
            EVENT.Event = 'on' + EVENT.Event;
            EVENT.Target.EVENT.Event = null;
        }
    }

    WireUpEventHandler(ListBoxDiv, 'contextmenu', function () { return false; });
    Base.appendChild(ListBoxDiv);
}
    




