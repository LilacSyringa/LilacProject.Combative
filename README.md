# LilacProject.Combative

Create different weaponry and effects that is modular as much as possible. Each additional functionality are added through what's called as additives.
The serialized objects or templates are all under the Constructs namespace.

# Prerequisite

Must support polymorphic serialization. 

# Effects

Effects allows you to execute a wide variety of things, expectedly projectiles or explosions.

## Usage

1. ```EffectInstance```
  - ```EffectInstance``` is the main container of any effects, it defines the basic functionalities of the effect. It has three subclasses by default that you should inherit from with different use cases:
    - ```DirectEffectInstance``` makes an ```EffectInstance``` without any sort of pooling. This is only used when the effect is not expected to be pooled at all, the effect is instantaneous that it has no chance to overlap with used multiple times in the same moement.
    - ```UnhandledEffectInstance``` makes multiple ```EffectInstance``` and pooled inside a ```UnhandledEffectInstancePool```. Use this when the effect is not instantaneous but does not require an ```iEffectHandle```.
    - ```HandledEffectInstance``` makes multiple ```EffectInstance``` like the previous but pooled in a ```HandledEffectInstancePool```, the only difference is that it contains a reference to a ```EffectHandlePool```. Each projection cycles through each in the pool as well as the ```EffectHandlePool```.
   
    - Both ```DirectEffectInstance``` and ```UnhandledEffectInstance``` has no right to contain additives that require components.
   
    - In the case that none of these fit your use case, it is possible to inherit from ```EffectInstance``` itself but read the usage of ```EffectProjectorBochord```.
   
  - Whichever ```EffectInstance``` you may use, you have access to **```EffectInstance.Additives```**, an object that contains references to multiple delegates tied to certain actions such as Hit or Update. **All** the operations of the Additve object must be used in any appropriate virtual methods of EffectInstance. Not doing so means that any additives added to the effect instance won't do anything at all.
   
2. ```IEffectProjector```
  - It is an interface that puts an effect into place. The interface can be used on the effect object itself like it ```DirectEffectInstance``` or through some pool managers like in ```UnhandledEffectInstancePool``` or ```HandledEffectInstancePool```.
  - You get references of these through the ```EffectProjectorBochord```.

3. ```EffectProjectorBochord```
  - ```EffectProjectorBochord``` is a **singleton** class that gives other classes access to ```IEffectProjector```, an interface that executes effects into place.
  - To use it, you must inherit a (sealed) class from it and set the ```EffectProjectorBochord.Instance``` value to itself.
  - Inheriting from it give you access to ```AddEffectProjector``` and you must add all necessary ```IEffectInstanceConstruct```. The base class already contains a dictionary and a getter that gets ```IEffectProjector``` by name by using the Indexer or the static method ```GetProjectors```. ```AddEffectProjector``` has two different overloads that requires you to provide an ```EffectPoolHandle``` when the ```EffectInstance``` turns out to be a ```HandledEffectInstance```, either provided directly or through a delegate. It is possible and should have different ```EffectPoolHandle``` for different kinds of effects as each effect may have different required components.
  - Optionally, you can set the value of ```EffectProjectorBochord.custom_projectorbuilder``` in the case that the ```EffectInstace``` you are using do not inherit from the default ```DirectEffectInstance```, ```HandledEffectInstance```, or```UnhandledEffectInstance```. The method provided is used to create an ```IEffectProjector``` out of the effect instance provided outside of the default three. You may use it to return itself or create some custom pooling mechanics.
   
4. ```IEffectHandle```
  - It is an interface that bridges between the actual game object and the effect. Both the EffectInstance or the EffectAdditive may use this interface.

5. ```EffectHandlePool```
  - This one contains a pool of ```IEffectHandle``` and used by ```HandledEffectInstancePool```. All the handles pooled inside contains the same components as it should as it could be cycled by different ```HandledEffectInstancePool```. As such, it is then suggested that you make multiple EffectHandlePool depending on the required components of each EffectInstance and its EffectAdditives, it is done so through a certain overload of ```EffectProjectorBochord.AddEffectProjector```.

6. ```EffectAdditive```
  - EffectAdditive add additional functionality to any effect. Though once the EffectInstance is fully initialised, I did not provide any means to add or remove any existing ```EffectAdditive```.
  - It contains many initialisation methods such as RecieveInputActions, RecieveAdditiveReference, RecieveProjectorReferences, and PostInitialize. The three of which are going to be provided by the EffectAdditiveConstruct with another set of virtual methods. Additionally with RecieveAdditiveReference also including an input for all of the Additives an effect have.
  - To get certain operations you also have to override the AdditiveActions and InputActions properties.
  - If the additive requires to have common set of properties between **each pool (each HandledEffectInstancePool or UnhandledEffectInstancePool) not global** there are three different ways.
    - ```ICommonCompositor```
      Interface that allows each additive in the pool to have a common object between them.
    - ```CommonAdditiveAttribute```
      An attribute that states that the additive itself is common to the entire pool.
      Adding this attribute exempts the additive from all initialisation methods.
    - Singleton
      This allows for a true common additive, not just in each pools.
      You are expected to provide the singleton implementation yourself. Just avoid using the initialisation methods.
      
  - Additionally, during the projection of an effect, ```IEffectProjector.Project``` has an optional parameter for additional initialisation of the effect before projection through a delegate, all additives are cycled on that delegate if the effect has any. Additives that have the attribute ```CommonAdditiveAttribute``` are skipped.
