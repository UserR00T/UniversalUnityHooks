[![Build Status](https://travis-ci.org/UserR00T/UniversalUnityHooks.svg?branch=master)](https://travis-ci.org/UserR00T/UniversalUnityHooks)

## Synopsis

UniversalUnityHooks aims to bring a adaptable modding API to any unity game. This is acomplished by hooking into methods found in the Assembly-CSharp.dll and modifying them while avoiding having to decompile and recompile.

## Code Example

```cs
public class YourClass
{
	[Hook("SvManager.StartServer")]
	public static void StartServer(SvManager man)
	{
		Debug.Log("Plugin Loaded!");
	}
}
```

In this example the game hooks the examples ``StartServer`` method in the ``SvManager`` Class, and prints Plugin Loaded when the server starts.
## Motivation

The inital motivation behind the project was to provide a way for server owners to develop plugins for the game Broke Protocol (https://brokeprotocol.com). But due to the way unity games are made, this program can be used on other games very easily.

## Installation
Goto [Releases](https://github.com/UserR00T/UniversalUnityHooks/releases).

Download the latest version.

Reference the .dll into your class library project. Make sure that the class that contains all the hooks is public as well as the hook methods themselfs.

Put your compiled DLLs in your ``root/Plugins`` folder. You may need to run ``UniversalUnityHooks.exe`` first to create that folder.

(Optional) I suggest adding this command to ``Post build event``: ``copy /Y "$(TargetFileName)" "..\..\..\..\Plugins\"``, assuming you keep your solution in ``root/Scripts``.

## API Reference

The program is fairly straight forward, to hook a method, simply put `[Hook("Class.Method")]` 

Then supply the instance as the first variable, and the rest of the variables as ``refs``.
So if I were to want to modify the ``Damage`` method inside of the ``SvPlayer`` class with variables ``DamageIndex``, ``Amount``, ``Attacker`` and ``Collider``, that would become
```cs
public class YourClass
{
    [Hook("SvPlayer.Damage")]
    public static bool Damage(SvPlayer player, ref DamageIndex type, ref float amount, ref ShPlayer attacker, ref Collider collider)
    {
      if (/*Your check for whatever here*/)
        return true; // Blocks the rest of the method
      else if (/*other check here*/)
        amount *= 2 // Multiply number by two.
      return false; // Allows the rest of the method to continue
    }
}
```
It'd compile it like this:
```cs
  // Token: 0x06000425 RID: 1061 RVA: 0x00018A38 File Offset: 0x00016C38
	public override void Damage(global::DamageIndex type, float amount, global::ShPlayer attacker, Collider collider)
	{
		if (YourClass.Damage(this, ref type, ref amount, ref attacker, ref collider))
		{
			return;
		}
    // Rest of damage method's code.
  }
```

## License

This code is licenced under the MIT Licence, [Information can be found here.](https://github.com/UserR00T/UniversalUnityHooks/blob/master/LICENSE)

## Limitations
Unfortunately there are some limitations;
- You can only inject your code at the start or end of the method. Nowhere in between. (To inject at the end, type ``[Hook("Class.Method", true)]``
- Injecting in a method of type IEnumerator does not work properly. 

## Credits

Ardivaba: https://github.com/Ardivaba

DeathByKorea: https://github.com/DeathByKorea