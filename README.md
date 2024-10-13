# [[SYR] Individuality (Continued)](https://steamcommunity.com/sharedfiles/filedetails/?id=3253596705)

![Image](https://i.imgur.com/buuPQel.png)

Update of Syrchaliss mod https://steamcommunity.com/sharedfiles/filedetails/?id=1497105115

- Romance option should now respect the replacement sexuality methods of the mod

![Image](https://i.imgur.com/pufA0kM.png)
	
![Image](https://i.imgur.com/Z4GOv8H.png)

## **Adds new traits, rebalances vanilla traits and shifts boring traits.**


![Image](https://i.imgur.com/jdqGJkN.png)

![Image](https://i.imgur.com/s8csNp7.png)

**What is Individuality?**


-  Adds exciting new traits that are not just stat bonuses (with C#)
-  Rebalances vanilla traits to give good traits a tiny downside and bad traits an upside
-  Gay, Bisexual, Asexual and Psychic Sensitvity are not traits anymore, they are now a stat inside the new individuality window
-  Also adds Confidence (romance attempt multiplier) and Body Weight as stats, which are randomized
-  Does small tweaks to the romance system (asexual pawns hook up with each other, beauty isn't so binary anymore, and more)




![Image](https://i.imgur.com/eiAAg6q.png)

**Details**
The sexuality, confidence, weight and psychic sensitvity can be changed anywhere you can open the menu. Enable edit mode and use left- or right-click on the fields. Mouse wheel is also supported.

**Content:**

- Several new profession-related traits are added
- Most new traits use C# for unique effects, not just boring stat changes
- Vanilla traits are rebalanced
- Good traits get a minor downside
- Bad traits get a minor upside
- You can select with how many traits pawns spawn
- Boring traits are removed
- A new menu inside the pawn - bio tab contains these traits now
- This includes: Sexuality, confidence (romance attempt multiplier), body weight and psychic sensitivity
- Sexuality ratios can be changed in the settings



**List of changes to vanilla traits:**


- Expert Shooter (better version of Careful Shooter)
- Slothful/Lazy pawns require much less joy
- HardWorker/Industrious need slightly more joy
- Slowpoke pawns need less rest, but are even slower
- FastWalker/Jogger pawns are even faster, but need slightly more rest
- Chemical Interest/Fascination get random inspirations and have less mental breaks
- Teetotaler has 20% global work speed
- Ugly/Staggeringly ugly pawns will not be considered "disfigured", even if they are
- Depressive pawns can only get Sad Wander and Hide in Room mental breaks
- Volatile pawns can only get Tantrum mental breaks
- Neurotic pawns don't break more easily, instead they get a random mood regularly, 
- Very neurotic pawns can get even more extreme moods
- Abrasive never get a mood debuff from being insulted/slighted themselves
- Greedy get special mood bonus for very impressive and better bedrooms
- Jealous don't mind wearing dead man's clothing or pawns getting banished/executed
- Wimps have very high toxic resistance, improved movespeed and can take a little more pain than vanilla
- Tough pawns are slower
- Pyromania has less mental breaks, gets random inspirations regularly



**Technical details:**

- Keen Eye: You have a varying chance to get 20% of a normal ore's yield. The chance for each ore is the same as when they are generated, so you will mostly get steel. The chance to get ore whenever you mine any sort of stone or ore depends on the tile the pawn is on. For flat it's a 20% chance, for small hills 12%, for large hills 10% and 5% for mountainous. If you have other hilliness types (e.g. impassable map maker) it defaults to 5%.
- Green Thumb: The mood change is between -5 and +5, from 0 to 9 beauty plants (that's flowers) in their bedroom. The thought changes at uneven numbers of plants (1, 3, 5, 7 and 9) respectively.
- Creative Thinker: The scaling with artistic is the same as intellectual, so you can get nearly twice the research speed (minus the base value)
- Slow Learner: The minimum value you can get is -50% if the pawn has 40 or less skill points spread across all skills. The maximum you can get is +150% for 140 points or more. However, your total global learning speed can be reduced further by other traits or by the stat base (e.g. you are playing a race that learns slowly).
- Perfectionist: Will simply add +1 to the quality level 50% of the time, like half a creativity inspiration. This should happen after any other modifier (from other mods for example).
- Animal Friend: The pawn treats every animal as if it had only 90% of it's actual wildness value. This means a megasloth has 87.3% wildness for a pawn with this trait instead of 97%. This makes taming these animals significantly more likely. The manhunter on failed tame chance is completely disabled.
- Steady Hands: Usually self-tending is done with 70% of the pawns medical tend quality. This trait bypasses this check and allows 100% of the medical tend quality to apply.
- Architect: It checks the material you construct something out of. Things that don't allow choosing a material will not be affected. If it has multiple ingredients the same rule applies - if you can choose the material it will apply, for example crematoriums need steel and components, but you can choose the stone type, so it will be affected.





![Image](https://i.imgur.com/BHPtNVt.png)

I highly suggest using **[Harvest Yield](https://steamcommunity.com/sharedfiles/filedetails/?id=1461790308)** with this mod. Otherwise the trait "Fortunate" becomes rather mediocre, **as animal gather yield** and **plant harvest yield** do not actually increase the amount you get beyond 100% without this mod.



![Image](https://i.imgur.com/x3y72Eg.png)

### **Savegames:**


- Adding: Safe
- Removing: Not safe



### **Incompatibilities:**


- Bad can be good - both mods add upsides to bad traits, you will get a messy mix of both and you will also get errors.





![Image](https://i.imgur.com/1YxHVGs.png)

[quote]### **[Syrchalis' Mods](https://steamcommunity.com/workshop/filedetails/?id=1474000866)**
[/quote]
Collection of my mods for RimWorld

![Image](https://i.imgur.com/PwoNOj4.png)



-  See if the the error persists if you just have this mod and its requirements active.
-  If not, try adding your other mods until it happens again.
-  Post your error-log using [HugsLib](https://steamcommunity.com/workshop/filedetails/?id=818773962) or the standalone [Uploader](https://steamcommunity.com/sharedfiles/filedetails/?id=2873415404) and command Ctrl+F12
-  For best support, please use the Discord-channel for error-reporting.
-  Do not report errors by making a discussion-thread, I get no notification of that.
-  If you have the solution for a problem, please post it to the GitHub repository.
-  Use [RimSort](https://github.com/RimSort/RimSort/releases/latest) to sort your mods

 

[![Image](https://img.shields.io/github/v/release/emipa606/SYRIndividuality?label=latest%20version&style=plastic&color=9f1111&labelColor=black)](https://steamcommunity.com/sharedfiles/filedetails/changelog/3253596705) | tags: trait rework, character customization
