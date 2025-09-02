// Audio recording functionality for Casa Bot
let mediaRecorder;
let audioChunks = [];
let audioStream;

window.startRecording = async function () {
    try {
        console.log('Starting audio recording...');
        
        // Request microphone access
        audioStream = await navigator.mediaDevices.getUserMedia({ audio: true });
        
        // Create MediaRecorder
        mediaRecorder = new MediaRecorder(audioStream);
        audioChunks = [];
        
        // Handle data available event
        mediaRecorder.ondataavailable = function (event) {
            audioChunks.push(event.data);
        };
        
        // Handle recording stop event
        mediaRecorder.onstop = function () {
            console.log('Recording stopped');
        };
        
        // Start recording
        mediaRecorder.start();
        console.log('Recording started');
        
    } catch (error) {
        console.error('Error starting recording:', error);
        throw new Error('Failed to access microphone: ' + error.message);
    }
};

window.stopRecording = function () {
    return new Promise((resolve, reject) => {
        try {
            if (!mediaRecorder || mediaRecorder.state === 'inactive') {
                resolve('');
                return;
            }
            
            mediaRecorder.onstop = function () {
                try {
                    // Stop all audio tracks
                    if (audioStream) {
                        audioStream.getTracks().forEach(track => track.stop());
                    }
                    
                    // Create blob from audio chunks
                    const audioBlob = new Blob(audioChunks, { type: 'audio/wav' });
                    
                    // Convert to base64
                    const reader = new FileReader();
                    reader.onloadend = function () {
                        const base64Audio = reader.result.split(',')[1]; // Remove data:audio/wav;base64, prefix
                        resolve(base64Audio);
                    };
                    reader.onerror = function () {
                        reject(new Error('Failed to convert audio to base64'));
                    };
                    reader.readAsDataURL(audioBlob);
                    
                } catch (error) {
                    console.error('Error processing recorded audio:', error);
                    reject(error);
                }
            };
            
            mediaRecorder.stop();
            
        } catch (error) {
            console.error('Error stopping recording:', error);
            reject(error);
        }
    });
};

// Utility function to scroll chat messages to bottom
window.scrollToBottom = function (element) {
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
};

// Check for microphone permissions on page load
window.checkMicrophonePermissions = async function () {
    try {
        const permissionStatus = await navigator.permissions.query({ name: 'microphone' });
        return permissionStatus.state; // 'granted', 'denied', or 'prompt'
    } catch (error) {
        console.log('Permissions API not supported, will prompt when needed');
        return 'unknown';
    }
};

// Initialize audio context and check browser compatibility
window.initializeAudio = function () {
    // Check for required APIs
    if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
        console.warn('Audio recording is not supported in this browser');
        return false;
    }
    
    if (!window.MediaRecorder) {
        console.warn('MediaRecorder is not supported in this browser');
        return false;
    }
    
    console.log('Audio recording initialized successfully');
    return true;
};