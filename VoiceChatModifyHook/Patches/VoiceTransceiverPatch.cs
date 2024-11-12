using HarmonyLib;
using NorthwoodLib.Pools;
using PlayerRoles.Voice;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;
using VoiceChat.Networking;
using static HarmonyLib.AccessTools;

namespace VoiceChatModifyHook.Patches;

[HarmonyPatch(typeof(VoiceTransceiver), nameof(VoiceTransceiver.ServerReceiveMessage))]
internal class VoiceTransceiverPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
        int index = newInstructions.FindIndex(instruction =>
            instruction.opcode == OpCodes.Callvirt
            && (MethodInfo)instruction.operand == Method(typeof(VoiceModuleBase), nameof(VoiceModuleBase.ValidateReceive))
        );
        index += 1;
        Collection<CodeInstruction> collection = new()
        {
            new(OpCodes.Ldarg_1),
            new(OpCodes.Ldfld, Field(typeof(VoiceMessage), nameof(VoiceMessage.Speaker))),
            new(OpCodes.Ldloc_3),
            CodeInstruction.Call(typeof(ModifyVoiceChat), nameof(ModifyVoiceChat.SCPChat)),
        };
        newInstructions.InsertRange(index, collection);

        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;

        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }
}
