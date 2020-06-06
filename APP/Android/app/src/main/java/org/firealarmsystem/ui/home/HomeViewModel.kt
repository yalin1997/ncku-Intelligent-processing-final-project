package org.firealarmsystem.ui.home

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel

class HomeViewModel : ViewModel() {

    private val _title = MutableLiveData<String>().apply {
        value = "火災警報系統"
    }

    val title: LiveData<String> = _title
    val gateways: MutableLiveData<Collection<String>> = MutableLiveData()
}