package org.firealarmsystem.ui.home

import android.content.Context
import android.graphics.Canvas
import android.graphics.Color
import android.graphics.Paint
import android.util.AttributeSet
import android.util.Log
import android.view.MotionEvent
import android.view.MotionEvent.*
import android.view.View
import kotlin.math.min
import kotlin.math.pow
import kotlin.math.sqrt

class AlarmButton : View {
    var mPaintOutline: Paint = Paint()
    var mPaintText: Paint = Paint()
    var mPaintBackground: Paint = Paint()
    var onPress = false
    var loopTime = 0

    private val radius: Float
        get() =  min(width , height) * 0.5f - 10f

    private val centerX :Float
        get() = width * 0.5f

    private val centerY :Float
        get() = height * 0.5f

    companion object{
        val colorNormal = Color.parseColor("#d32f2f")
        val colorPress = Color.parseColor("#ff6659")
        val backgroundNormal = Color.parseColor("#ffffff")
        val backgroundPress = Color.parseColor("#ffa4a2")
        const val animationFactor = 20
    }

    constructor(context : Context) : super(context) {
    }

    constructor(context:Context , attrs : AttributeSet)
            : super(context , attrs){
    }

    constructor(context: Context, attrs: AttributeSet ,  defStyleAttr: Int, defStyleRes: Int)
            :super(context , attrs , defStyleAttr , defStyleRes){
    }

    init {
        mPaintOutline.color = colorNormal
        mPaintOutline.style = Paint.Style.STROKE
        mPaintOutline.isAntiAlias = true
        mPaintBackground.color = backgroundNormal
        mPaintBackground.style = Paint.Style.FILL
        mPaintText.textAlign = Paint.Align.CENTER
        mPaintText.color = colorNormal

        setOnTouchListener { view, event ->
            when(event.action){
                ACTION_DOWN -> {
                    if(inCircle(event.x , event.y)){
                        mPaintOutline.color = colorPress
                        mPaintBackground.color = backgroundPress
                        onPress = true
                        view.invalidate()
                    }
                    return@setOnTouchListener true
                }
                ACTION_UP ->{
                    mPaintOutline.color = colorNormal
                    mPaintBackground.color = backgroundNormal
                    onPress = false
                    loopTime = 0
                    view.invalidate()
                    if(inCircle(event.x , event.y))
                        view.performClick()
                    return@setOnTouchListener false
                }
            }
            false
        }
    }

    override fun onDraw(canvas: Canvas) {
        super.onDraw(canvas)

        mPaintOutline.strokeWidth = radius * 0.05f
        mPaintText.textSize = radius * 0.2f

        canvas.drawCircle(centerX , centerY , radius , mPaintBackground)
        canvas.drawCircle(centerX , centerY , radius , mPaintOutline)
        canvas.drawText("發出火災警報" , centerX , centerY , mPaintText)
    }

    private fun inCircle(x:Float , y:Float) : Boolean {
        return sqrt( (x - centerX).pow(2) + (y - centerY).pow(2) ) < radius
    }
}