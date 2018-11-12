using MIG.Scripts.Character;

namespace MIG.Scripts.Dtos
{
    public struct PlayerDto
    {
        public Team PlayerTeam;
        public PhotonPlayer PlayerPhoton;
        public bool Ready;

        public PlayerDto(Team playerTeam, PhotonPlayer playerPhoton)
        {
            Ready = false;
            PlayerTeam = playerTeam;
            PlayerPhoton = playerPhoton;
        }
    }
}