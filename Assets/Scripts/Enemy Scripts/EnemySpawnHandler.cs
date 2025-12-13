using System;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Text;

//it is not necessary to split this into 2 files. EnemySpawnContainer is only used as a data storage and should not be acessed outside the handler.
public class EnemySpawnHandler
{
    //used elapsed...
    private float elapsed;
    private bool is_spawning;
    private bool is_awaiting_wave_change;
    private int active_wave;
    private EnemySpawnContainer[] spawndata = new EnemySpawnContainer[0];
    private EnemySpawnContainer[] is_active = new EnemySpawnContainer[0];
    private LevelInstance level_reference;
    public EnemySpawnHandler(LevelInstance l_ref, string f_path) //+file to read from
    {
        level_reference = l_ref;
        StreamReader ef_reader;
        try
        {
            var fdat = Resources.Load<TextAsset>("TSTD Data - Level 1.csv");

            var x = new MemoryStream(Encoding.UTF8.GetBytes(fdat.text));

            ef_reader = new StreamReader(x);
            Debug.Log("Enemy spawn data loaded.");

            string line = ef_reader.ReadLine();
            while (!line.Split(',')[0].Equals("Enemy Spawn Data"))
            {
                line = ef_reader.ReadLine();
            }
            line = ef_reader.ReadLine();

            string[] loading_container_properties = line.Split(',');

            while ((line = ef_reader.ReadLine()) != null)
            {
                string[] spawn_inst_prop = line.Split(',');
                EnemySpawnContainer spawn_inst_container = new EnemySpawnContainer();
                for (int i = 0; i < loading_container_properties.Length; i++)
                {
                    var set_prop = typeof(EnemySpawnContainer).GetProperty(loading_container_properties[i]);
                    if (set_prop == null)
                    {
                        Debug.Log("The property field (" + loading_container_properties[i] + ") does not exist for EnemySpawnContainer.");
                        continue;
                    }
                    switch (set_prop.PropertyType.ToString())
                    {
                        case "System.Int32":
                            set_prop.SetValue(spawn_inst_container, Int32.Parse(spawn_inst_prop[i]));
                            break;
                        case "System.Boolean":
                            set_prop.SetValue(spawn_inst_container, Boolean.Parse(spawn_inst_prop[i]));
                            break;
                        case "System.Single":
                            set_prop.SetValue(spawn_inst_container, Single.Parse(spawn_inst_prop[i]));
                            break;
                        case "System.String":
                            set_prop.SetValue(spawn_inst_container, spawn_inst_prop[i]);
                            break;
                        default:
                            Debug.Log("The property (" + loading_container_properties[i] + ") is not of recognized type.");
                            break;
                    }
                }
                spawn_inst_container.initContainer();
                spawndata = spawndata.Append(spawn_inst_container).ToArray(); //this can be optimized
            }
        }
        catch (Exception e) {
            Debug.Log("Enemy spawn file for level could not be read:");
            Debug.Log(e.Message);
        }

        this.active_wave = -1;
        is_spawning = true;
        change_wave();
    }

    public void pause_Spawns() => is_spawning = false;
    public void resume_Spawns() => is_spawning = true;


    public int[] get_spawn_data() => new int[1]; //returns a set of spawn data of enemies in each wave
    public int enemies_until_next_wave() => 0; //0 indicates that wave summoner unit active
    public int enemies_in_current_wave() => 0;
    public int enemies_remaining_total() => 0;
    public int waves_remaining_total() => 0;

    /// <summary>
    /// Calls the next wave
    /// </summary>
    /// <returns>True - if wave was sucessfully called
    /// False - if wave does not call?</returns>
    public bool change_wave() //gotta make the enemy call this, or autocall when all enemies in wave has been elminitated and more waves remain
    {
        //check if can wavechange, if so:
        active_wave++;

        int prev_remaining_container = 0;
        foreach (var container in is_active) if (container.is_completed == false) prev_remaining_container++;
        int new_add_container = 0;
        foreach (var container in spawndata) if (container.spawn_wave == active_wave) new_add_container++;
        
        EnemySpawnContainer[] new_active = new EnemySpawnContainer[prev_remaining_container + new_add_container];
        int new_active_cntr = 0;
        foreach (var container in is_active) if (container.is_completed == false)
            {
                new_active[new_active_cntr] = container;
                new_active_cntr++;
            }
        foreach (var container in spawndata) if (container.spawn_wave == active_wave)
            {
                new_active[new_active_cntr] = container;
                new_active_cntr++;
            }
        is_active = new_active;
        Debug.Log("Beginning wave " + active_wave);
        return true;
    }
    public void update()
    {
        if (!is_spawning) return;

        elapsed += Time.deltaTime;

        foreach (EnemySpawnContainer container in is_active)
        {
            if (container.is_completed == true) continue;
            container.cooldown -= Time.deltaTime;
            if (container.cooldown <= 0)
            {
                container.cooldown = container.spawn_interval;

                //workaround placeholder - will be changed
                level_reference.SpawnEnemy(container.EnemyID, 0);
                //...LevelInstance.SpawnEnemy(type,scale,forcewave_change: true);

                container.count_compl++;
                if (container.count_compl >= container.count){
                    container.is_completed = true;
                    //check wave change completion if container.spawnwave = currentwave
                }
            }
        }
    }
}

public class EnemySpawnContainer
{
    //loaded info
    public int count {  get; set; }
    public float inital_spawn_delay { get; set; }
    public float spawn_interval { get; set; } //should be >0
    public string EnemyID { get; set; } //verify that type exists on creation
    public int spawn_wave {  get; set; }
    public int scale { get; set; }

    //determined info?
    public bool is_wave_change_ready_on_completion; //maybe have this be inherited when verified

    //operating info
    public bool is_completed;
    public float start_time;
    public int count_compl;
    public float cooldown;

    public void initContainer()
    {
        is_completed = false;
        count_compl = 0;
        cooldown = inital_spawn_delay;
    }

}
