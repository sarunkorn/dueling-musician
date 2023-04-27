using System.IO;

namespace SmileProject.SpaceInvader.Sounds
{
    public class MixerGroup : StringEnum<MixerGroup>
    {
        public const string MainMixerKey = "SoundMixer";
        public MixerGroup(string value) : base(value)
        {
        }

        public static readonly MixerGroup BGM = new MixerGroup("BGM");
        public static readonly MixerGroup SoundEffect = new MixerGroup("SoundEffect");
    }

    public sealed class GameSoundKeys : SoundKeys
    {
        public GameSoundKeys(string value, string assetKey, string mixerKey) : base(value, assetKey, mixerKey)
        {
        }
        
        #region BGM
        public static readonly GameSoundKeys DefaultBGM = new GameSoundKeys(nameof(DefaultBGM), "DefaultBGM", MixerGroup.BGM.ToString());
        public static readonly GameSoundKeys RockBGM = new GameSoundKeys(nameof(RockBGM), "RockBGM", MixerGroup.BGM.ToString());
        public static readonly GameSoundKeys DJBGM = new GameSoundKeys(nameof(DJBGM), "DJBGM", MixerGroup.BGM.ToString());
        public static readonly GameSoundKeys JazzBGM = new GameSoundKeys(nameof(JazzBGM), "JazzBGM", MixerGroup.BGM.ToString());
        public static readonly GameSoundKeys MaestroBGM = new GameSoundKeys(nameof(MaestroBGM), "MaestroBGM", MixerGroup.BGM.ToString());
        #endregion
        
        #region Sound effects
        public static readonly GameSoundKeys PlayerJoined = new GameSoundKeys(nameof(PlayerJoined), "PlayerJoined", MixerGroup.SoundEffect.ToString());
        public static readonly GameSoundKeys GameStart = new GameSoundKeys(nameof(GameStart), "GameStart", MixerGroup.SoundEffect.ToString());
        public static readonly GameSoundKeys Dash = new GameSoundKeys(nameof(Dash), "Dash", MixerGroup.SoundEffect.ToString());
        public static readonly GameSoundKeys MicGrab = new GameSoundKeys(nameof(MicGrab), "MicGrab", MixerGroup.SoundEffect.ToString());
        public static readonly GameSoundKeys Taunt = new GameSoundKeys(nameof(Taunt), "Taunt", MixerGroup.SoundEffect.ToString());
        public static readonly GameSoundKeys Victory = new GameSoundKeys(nameof(Victory), "Victory", MixerGroup.SoundEffect.ToString());
        #endregion
    }
}