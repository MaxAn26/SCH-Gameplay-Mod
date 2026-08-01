using System.Collections;
using BaseMod.Core;
using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;
using static BaseMod.Core.ModConfig;

namespace Solas.GameplayMod.Mods;
internal class FuckMeMod
{
    #region Configuration
    internal static MelonPreferences_Entry<bool> Enabled;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    internal static bool NotAvailable { get; private set; } = false;
    #endregion

    #region Storage
    internal static SexSystem SexSystem { get; set; }
    internal static Rigidbody PlayerRigitbody;
    #endregion

    internal static void Load(ModConfig config)
    {
        try
        {
            Enabled = config.Entry(nameof(FuckMeMod), nameof(Enabled), false,
                "Activates the modification", new AcceptableValueList<bool>([true, false]));
        }
        catch (Exception ex)
        {
            Core.LogError(ex);
        }
    }

    internal static void Apply(SexSystem sexSystem)
    {
        try
        {
            if (!Enabled.Value || SceneManager.GetActiveScene().buildIndex <= 4)
            {
                return;
            }

            SexSystem = sexSystem;
            if (SexSystem.Player.TryGetComponentWithCast(out Rigidbody rigitbody))
            {
                PlayerRigitbody = rigitbody;
                MelonCoroutines.Start(FuckMeEnumerator());
            }

        }
        catch (Exception ex)
        {
            Core.LogError(ex);
        }
    }

    private static IEnumerator FuckMeEnumerator()
    {
        do
        {
            if (SexSystem is null || PlayerRigitbody is null || SexSystem.WasCollected || PlayerRigitbody.WasCollected)
            {
                yield break;
            }

            if (SexSystem.GameOver || SceneManager.GetActiveScene().buildIndex < 4)
            {
                yield break;
            }

            if (SexSystem.Sexstatus is SEXSTATUS.Fucking)
            {
                NotAvailable = true;
            }
            else if (SexSystem.Sexstatus is SEXSTATUS.Idle && SexSystem.playerHealthSystem.CurrentAr == SexSystem.playerHealthSystem.MaxAr)
            {
                if (!NotAvailable)
                {
                    NotAvailable = false;
                }
                else
                {
                    int chance = 5;
                    chance += SexSystem.playerHealthSystem.CurrentEc / 10;
                    if (chance > 40)
                    {
                        chance = 40;
                    }

                    if (RandomUtils.Chance(chance))
                    {
                        SexSystem.console.ConsoleWrite("Fuck me");
                        SexSystem.CanMove = false;
                        PlayerRigitbody.velocity = Vector3.zero;
                        PlayerRigitbody.angularVelocity = Vector3.zero;
                        SexSystem.Taunt();
                        yield return new WaitForSeconds(5f);
                        SexSystem.CanMove = true;
                    }
                }
            }
            yield return new WaitForSeconds(10f);
        } while (true);
    }
}
