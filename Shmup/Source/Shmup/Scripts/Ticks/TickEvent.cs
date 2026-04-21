using Godot;

// Author : Julien Fournier

namespace Com.IsartDigital.Chromaberation
{

    public partial class TickEvent : AudioStreamPlayer
    {
        protected AudioEffectSpectrumAnalyzerInstance spectrum;

        protected const float FREQ_MAX = 11050.0f;
        protected const float MIN_DB = 60.0f;
        protected int busIndex;


        //AnimationPlayer anim;
        protected RandomNumberGenerator rand = new RandomNumberGenerator();

        // timer pour éviter de trigger des events plusieurs fois pendant la durée d'un click
        // un genre de cooldown
        protected float timer = 0.0f;

        // Seuil de décibel auquel doit être déclenché l'event
        protected const float ENERGY_THRESHOLD = 0.001f;

        // quand le son dépase ENERGY_THRESOLD (seuil de décibel)
        // réglez TIMER_INTERVAL pour qu'il n'écoute plus le dépassement de db avant
        // avant TIMER_INTERVAL secondes 
        // attention si vous avez certain clicks trop rapprochés certains pourraient
        // être ignoré avec un interval trop long
        // En revanche un interval trop court générera des events non désirés
        protected const float TIMER_INTERVAL = 0.2f;


        public override void _Ready()
        {

            busIndex = AudioServer.GetBusIndex(Name.ToString());
            spectrum = AudioServer.GetBusEffectInstance(busIndex, 0) as AudioEffectSpectrumAnalyzerInstance;

            // empêche le bug d'energy résiduelle.
            Finished += QueueFree;

        }


        public override void _Process(double pDelta)
        {
            base._Process(pDelta);

            Vector2 lMagnitude = spectrum.GetMagnitudeForFrequencyRange(0, FREQ_MAX, (int)AudioEffectSpectrumAnalyzerInstance.MagnitudeMode.Average);

            float lEnergy = Mathf.Clamp((MIN_DB + LinearToDb(lMagnitude.Length())) / MIN_DB, 0, 1);


            if (lEnergy > ENERGY_THRESHOLD && timer >= TIMER_INTERVAL)
            {
                timer = 0;
                OnBeat();
            }

            timer += (float)pDelta;
        }

        protected virtual void OnBeat()
        {
        }


        protected float LinearToDb(float pLinear)
        {
            // Conversion de l'échelle linéaire en dB
            if (pLinear <= 0.0001f) return -MIN_DB; // Retourne une valeur de dB très basse si 'linear' est presque nul.
            return 20.0f * Mathf.Log(pLinear) / Mathf.Log(10.0f);
        }
    }
}
