import mne

raw = mne.io.read_raw_eeglab("S1_2_task2as int ICAout 6 20 23.set", preload=True)
#print(raw.info)
#print(raw.ch_names)
raw.plot()

events = mne.events_from_annotations(raw)

#events = mne.find_events(raw, shortest_event=0 )#, stim_channel="STI101")
print(events)
#input()
