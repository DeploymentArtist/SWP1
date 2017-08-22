Sub InitializeLocation (DropDownID, MDTListProperty)
  ' Get drop-down list
  Dim oDropDown
  Set oDropDown = document.getElementByID(DropDownID)

  ' Get MDT Property
  Dim sMDTProp
  sMDTProp = oEnvironment.Item(DropDownID)

  ' Iterate through MDT list property
  For Each sItem In oEnvironment.ListItem(MDTListProperty)
    ' Create new option element
    Dim oOption
    Set oOption = document.createElement("OPTION")
    oOption.text = sItem
    oOption.value = sItem

    ' If item equals MDT property
    ' mark the current option as selected
    If sMDTProp = sItem Then
      oOption.selected = True
    End If

    ' Add option to DropDown
    oDropDown.Add(oOption) 
  Next
End Sub

Function ValidateLocation (DropDownID)
  Dim oDropDown
  Set oDropDown = document.getElementByID(DropDownID)
  bFound = False
  For Each oOption In oDropDown
    If oOption.Selected Then
      bFound = True
      Exit For
    End If
  Next
  ValidateLocation = bFound
End Function
