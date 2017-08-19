using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class
MouseController : MonoBehaviour
{
  public bool mouseInUse = false;
  public enum ControlMode
  {
    None,
    UpgradeMenu
  } public ControlMode controlMode = ControlMode.None;
}
