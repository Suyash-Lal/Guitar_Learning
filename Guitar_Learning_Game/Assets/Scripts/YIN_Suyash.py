#!/usr/bin/env python
# -*- coding: utf-8 -*-

import numpy as np
import matplotlib.pyplot as plt
from scipy.signal import fftconvolve
import os
from os import sep
from audio_processing import *
import time
from scipy import stats

def differenceFunction_original(x, N, tau_max):
    """
    Compute difference function of data x. This corresponds to equation (6) in [1]

    Original algorithm.

    :param x: audio data
    :param N: length of data
    :param tau_max: integration window size
    :return: difference function
    :rtype: list
    """
    df = [0] * tau_max
    for tau in range(1, tau_max):
         for j in range(0, N - tau_max):
             tmp = int(x[j] - x[j + tau])
             df[tau] += tmp * tmp
    return df

def differenceFunction_scipy(x, N, tau_max):
    """
    Compute difference function of data x. This corresponds to equation (6) in [1]

    Faster implementation of the difference function.
    The required calculation can be easily evaluated by Autocorrelation function or similarly by convolution.
    Wiener–Khinchin theorem allows computing the autocorrelation with two Fast Fourier transforms (FFT), with time complexity O(n log n).
    This function use an accellerated convolution function fftconvolve from Scipy package.

    :param x: audio data
    :param N: length of data
    :param tau_max: integration window size
    :return: difference function
    :rtype: list
    """
    x = np.array(x, np.float64)
    w = x.size
    x_cumsum = np.concatenate((np.array([0]), (x * x).cumsum()))
    conv = fftconvolve(x, x[::-1])
    tmp = x_cumsum[w:0:-1] + x_cumsum[w] - x_cumsum[:w] - 2 * conv[w - 1:]
    return tmp[:tau_max + 1]


def differenceFunction(x, N, tau_max):
    """
    Compute difference function of data x. This corresponds to equation (6) in [1]

    Fastest implementation. Use the same approach than differenceFunction_scipy.
    This solution is implemented directly with Numpy fft.


    :param x: audio data
    :param N: length of data
    :param tau_max: integration window size
    :return: difference function
    :rtype: list
    """

    x = np.array(x, np.float64)
    w = x.size
    tau_max = min(tau_max, w)
    x_cumsum = np.concatenate((np.array([0.]), (x * x).cumsum()))
    size = w + tau_max
    p2 = (size // 32).bit_length()
    nice_numbers = (16, 18, 20, 24, 25, 27, 30, 32)
    size_pad = min(x * 2 ** p2 for x in nice_numbers if x * 2 ** p2 >= size)
    fc = np.fft.rfft(x, size_pad)
    conv = np.fft.irfft(fc * fc.conjugate())[:tau_max]
    return x_cumsum[w:w - tau_max:-1] + x_cumsum[w] - x_cumsum[:tau_max] - 2 * conv



def cumulativeMeanNormalizedDifferenceFunction(df, N):
    """
    Compute cumulative mean normalized difference function (CMND).

    This corresponds to equation (8) in [1]

    :param df: Difference function
    :param N: length of data
    :return: cumulative mean normalized difference function
    :rtype: list
    """

    cmndf = df[1:] * range(1, N) / (np.cumsum(df[1:]).astype(float) + 1e-10)  # scipy method
    return np.insert(cmndf, 0, 1)



def getPitch(cmdf, tau_min, tau_max, harmo_th=0.1):
    """
    Return fundamental period of a frame based on CMND function.

    :param cmdf: Cumulative Mean Normalized Difference function
    :param tau_min: minimum period for speech
    :param tau_max: maximum period for speech
    :param harmo_th: harmonicity threshold to determine if it is necessary to compute pitch frequency
    :return: fundamental period if there is values under threshold, 0 otherwise
    :rtype: float
    """
    tau = tau_min
    while tau < tau_max:
        if cmdf[tau] < harmo_th:
            while tau + 1 < tau_max and cmdf[tau + 1] < cmdf[tau]:
                tau += 1
            return tau
        tau += 1

    return 0    # if unvoiced



def compute_yin(sig, sr, dataFileName=r"C:\GitHub Repos\Thesis-Guitar_Project\Guitar_Learning\Guitar_Learning_Game\Assets\Scripts\Player_Recordings\recording0.wav", w_len=512, w_step=256, f0_min=80, f0_max=500, harmo_thresh=0.1):
    """

    Compute the Yin Algorithm. Return fundamental frequency and harmonic rate.

    :param sig: Audio signal (list of float)
    :param sr: sampling rate (int)
    :param w_len: size of the analysis window (samples)
    :param w_step: size of the lag between two consecutives windows (samples)
    :param f0_min: Minimum fundamental frequency that can be detected (hertz)
    :param f0_max: Maximum fundamental frequency that can be detected (hertz)
    :param harmo_tresh: Threshold of detection. The yalgorithmù return the first minimum of the CMND fubction below this treshold.

    :returns:

        * pitches: list of fundamental frequencies,
        * harmonic_rates: list of harmonic rate values for each fundamental frequency value (= confidence value)
        * argmins: minimums of the Cumulative Mean Normalized DifferenceFunction
        * times: list of time of each estimation
    :rtype: tuple
    """

    print('Yin: compute yin algorithm')
    tau_min = int(sr / f0_max)
    tau_max = int(sr / f0_min)

    timeScale = range(0, len(sig) - w_len, w_step)  # time values for each analysis window
    times = [t/float(sr) for t in timeScale]
    frames = [sig[t:t + w_len] for t in timeScale]

    pitches = [0.0] * len(timeScale)
    harmonic_rates = [0.0] * len(timeScale)
    argmins = [0.0] * len(timeScale)

    for i, frame in enumerate(frames):

        #Compute YIN
        df = differenceFunction(frame, w_len, tau_max)
        cmdf = cumulativeMeanNormalizedDifferenceFunction(df, tau_max)
        p = getPitch(cmdf, tau_min, tau_max, harmo_thresh)

        #Get results
        if np.argmin(cmdf)>tau_min:
            argmins[i] = float(sr / np.argmin(cmdf))
        if p != 0: # A pitch was found
            pitches[i] = float(sr / p)
            harmonic_rates[i] = cmdf[p]
        else: # No pitch, but we compute a value of the harmonic rate
            harmonic_rates[i] = min(cmdf)


    if dataFileName is not None:
        np.savez(dataFileName, times=times, sr=sr, w_len=w_len, w_step=w_step, f0_min=f0_min, f0_max=f0_max, harmo_thresh=harmo_thresh, pitches=pitches, harmonic_rates=harmonic_rates, argmins=argmins)
        print('\t- Data file written in: ' + dataFileName)

    return pitches, harmonic_rates, argmins, times

def calculate_average_frequency(pitches, times):
    """Calculate the most dominant (longest played) frequency from detected pitches, excluding zeros (unvoiced segments)."""
    assert len(pitches) == len(times), "The lengths of pitches and times should be the same."

    # Dictionary to hold the cumulative duration of each frequency
    frequency_durations = {}

    # Process each pitch with its corresponding time
    for i in range(1, len(pitches)):
        if pitches[i] > 0:  # Exclude zero values
            # Calculate the duration for which this pitch was held
            duration = times[i] - times[i-1]

            # If the pitch is already in the dictionary, add the duration, else set it
            if pitches[i] in frequency_durations:
                frequency_durations[pitches[i]] += duration
            else:
                frequency_durations[pitches[i]] = duration

    # Find the frequency with the maximum cumulative duration
    if frequency_durations:
        longest_played_frequency = max(frequency_durations, key=frequency_durations.get)
        return longest_played_frequency
    else:
        return 0


def find_closest_guitar_note(frequency):
    # Define the notes and their frequencies in one octave
    base_notes = ['E', 'F', 'F#', 'G', 'G#', 'A', 'A#', 'B', 'C', 'C#', 'D', 'D#', 'E (next octave)']
    base_frequencies = [82, 87, 93, 98, 104, 110, 117, 124, 131, 139, 147, 156, 165]

    # Lists to hold extended frequencies and notes
    extended_frequencies = []
    extended_notes = []

    # Generate frequencies and notes for multiple octaves
    for octave in range(5):  # This example extends up to the fifth octave
        new_frequencies = [f * (2 ** octave) for f in base_frequencies]
        extended_frequencies.extend(new_frequencies)
        if octave < 4:
            extended_notes.extend(base_notes)
        else:
            # Avoid duplicating the last E note in the final octave expansion
            extended_notes.extend(base_notes[:-1])

    # Find the closest frequency
    closest_freq = min(extended_frequencies, key=lambda x: abs(x - frequency))
    note_index = extended_frequencies.index(closest_freq)
    return extended_notes[note_index], closest_freq  # Return the closest note and its frequency

def write_closest_note_to_file(frequency, file_path):
    closest_note, closest_frequency = find_closest_guitar_note(frequency)
    with open(file_path, 'w') as file:
        # file.write(f"The closest guitar note to {frequency} Hz is {closest_note} ({closest_frequency} Hz).\n")
        file.write(closest_note)

def main(audioFileName = r"C:\GitHub Repos\Thesis-Guitar_Project\Guitar_Learning\Guitar_Learning_Game\Assets\Scripts\Player_Recordings\recording0.wav", w_len=1024, w_step=256, f0_min=80, f0_max=500, harmo_thresh=0.83, audioDir="./", dataFileName=None, verbose=4):
    """
    Run the computation of the Yin algorithm on a example file.

    Write the results (pitches, harmonic rates, parameters ) in a numpy file.

    :param audioFileName: name of the audio file
    :type audioFileName: str
    :param w_len: length of the window
    :type wLen: int
    :param wStep: length of the "hop" size
    :type wStep: int
    :param f0_min: minimum f0 in Hertz
    :type f0_min: float
    :param f0_max: maximum f0 in Hertz
    :type f0_max: float
    :param harmo_thresh: harmonic threshold
    :type harmo_thresh: float
    :param audioDir: path of the directory containing the audio file
    :type audioDir: str
    :param dataFileName: file name to output results
    :type dataFileName: str
    :param verbose: Outputs on the console : 0-> nothing, 1-> warning, 2 -> info, 3-> debug(all info), 4 -> plot + all info
    :type verbose: int
    """
    
    if audioDir is not None:
        audioFilePath = audioDir + sep + audioFileName
    else:
        audioFilePath = audioFileName

    audioFilePath = os.path.join(audioDir, audioFileName)

    sr, sig = audio_read(audioFilePath, formatsox=False)

    timeScale = range(0, len(sig) - w_len, w_step)  # time values for each analysis window
    times = [t/float(sr) for t in timeScale]

    start = time.time()
    pitches, harmonic_rates, argmins, times = compute_yin(sig, sr, dataFileName, w_len, w_step, f0_min, f0_max, harmo_thresh)
    end = time.time()
    print("Yin computed in: ", end - start)

    duration = len(sig)/float(sr)

    
    # After computing the yin pitches
    average_frequency = calculate_average_frequency(pitches, times)
    print("Average Frequency:", average_frequency)
    closest_note, closest_frequency = find_closest_guitar_note(average_frequency)
    print(f"The closest guitar note to the average frequency {average_frequency} Hz is {closest_note} ({closest_frequency} Hz).")

    # Write closest note to a text file
    output_file_path = os.path.join(audioDir, r"C:\GitHub Repos\Thesis-Guitar_Project\Guitar_Learning\Guitar_Learning_Game\Assets\Scripts\closest_note.txt")  # You can specify a different path if needed
    write_closest_note_to_file(average_frequency, output_file_path)

    if verbose >3:
        ax4 = plt.subplot(4,1,2)
        ax4.plot([float(x) * duration / len(argmins) for x in range(0, len(argmins))], argmins)
        ax4.set_title('Index of minimums of CMND')
        ax4.set_ylabel('Frequency (Hz)')
        ax4.set_xlabel('Time (seconds)')
        plt.show()

    return closest_note

if __name__ == '__main__':
    main()
