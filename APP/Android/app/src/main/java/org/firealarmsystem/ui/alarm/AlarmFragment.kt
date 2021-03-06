package org.firealarmsystem.ui.alarm

import android.media.RingtoneManager
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.appcompat.app.AlertDialog
import androidx.fragment.app.Fragment
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.ViewModelProviders
import org.firealarmsystem.R
import org.firealarmsystem.ui.home.HomeViewModel


class AlarmFragment : Fragment() {

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        val root = inflater.inflate(R.layout.fragment_alarm , container, false)

        return View(inflater.context)
    }

    override fun onStart() {
        super.onStart()

        val context = this.context!!

        val builder: AlertDialog.Builder = AlertDialog.Builder(context)
        builder.setMessage("")
            .setPositiveButton("確認"
            ) { dialogInterface, _ ->
                dialogInterface.cancel()
            }
            .setMessage("發生火災!!")
            .setTitle("塊陶阿")

        val notification = RingtoneManager.getDefaultUri(RingtoneManager.TYPE_NOTIFICATION)
        val r = RingtoneManager.getRingtone(
            context.applicationContext,
            notification)
        r.play()

        val dialog = builder.create()

        val homeViewModel = activity?.run {
            ViewModelProviders.of(this).get(HomeViewModel::class.java)
        } ?: throw Exception("Invalid Activity")

        homeViewModel.isAlarm.observe(this , Observer { flag ->
            if(flag){
                dialog.show()
            }
        })
    }


}