using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Voice
{
    public class VoiceParticipantState
    {
        public string UserId { get; set; } = string.Empty;
        public bool IsSelfMuted { get; set; }
        public bool IsForceMuted { get; set; }
        public bool IsSpeaking { get; set; }
    }
}
