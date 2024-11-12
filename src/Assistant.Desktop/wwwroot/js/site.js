let mediaRecorder;
let audioChunks = [];

window.scrollToBottom = (element) => {
    setTimeout(() => {
        element.scrollTop = element.scrollHeight;
    }, 500)
};

function startRecording() {
    audioChunks = [];
    navigator.mediaDevices.getUserMedia({ audio: true })
        .then(stream => {
            mediaRecorder = new MediaRecorder(stream);
            mediaRecorder.ondataavailable = event => {
                audioChunks.push(event.data);
            };
            mediaRecorder.start();
        });
}

function stopRecording() {
    return new Promise(resolve => {
        mediaRecorder.onstop = async () => {
            const audioBlob = new Blob(audioChunks, { type: mediaRecorder.mimeType });
            const mp3Blob = await convertToMp3(audioBlob);
            audioChunks = [];
            const reader = new FileReader();
            reader.onloadend = () => {
                resolve(reader.result); // Base64 string
            };
            reader.readAsDataURL(mp3Blob); // Read audio as Base64
        };
        mediaRecorder.stop();
    });
}

async function convertToMp3(audioData) {
    const wav = await audioData.arrayBuffer();
    const audioContext = new AudioContext();
    const audioBuffer = await audioContext.decodeAudioData(wav);

    // Configure MP3 encoder
    const mp3encoder = new lamejs.Mp3Encoder(1, audioBuffer.sampleRate, 128);
    const channels = audioBuffer.numberOfChannels;
    const samples = new Float32Array(audioBuffer.length);
    audioBuffer.copyFromChannel(samples, 0, 0);

    // Convert to 16-bit samples
    const sampleSize = 1152;
    const mp3Data = [];
    for (let i = 0; i < samples.length; i += sampleSize) {
        const sampleChunk = samples.subarray(i, i + sampleSize);
        const intSamples = new Int16Array(sampleChunk.length);
        for (let j = 0; j < sampleChunk.length; j++) {
            intSamples[j] = sampleChunk[j] * 32767;
        }
        const mp3buf = mp3encoder.encodeBuffer(intSamples);
        if (mp3buf.length > 0) {
            mp3Data.push(mp3buf);
        }
    }

    // Finalize encoding
    const end = mp3encoder.flush();
    if (end.length > 0) {
        mp3Data.push(end);
    }

    // Combine MP3 chunks
    const blob = new Blob(mp3Data, { type: 'audio/mp3' });
    return blob;
}